unit HeatMapGen;

interface

uses
  Windows, SysUtils, Classes, Graphics, Math, PngImage, SlidingFilters;

type
  PRgbPalette = ^TRgbPalette;
  TRgbPalette =  array[Byte] of TRgbQuad;

  THeatMapGenerator = class
  private
    FInFileName, FOutFileName: TFileName;
    Fields: TStringList;

    FreqStart, FreqEnd: integer;
    FreqStep: Single;
    TimeStart, TimeEnd: string;
    TimeStr: string;

    BinCnt: integer;
    Data: array of TSingleArray;
    Spect: TSingleArray;

    FInfo: string;

    procedure AnalyzeFile;
    procedure ImportText;
    procedure SubtractNoise2D;
    procedure SubtractNoise1D;
    procedure SubtractNoiseConst;
    procedure MakeImage(PngFormat: boolean);
    function InfoToJson: string;
    procedure ProcessLine(S: string);
    procedure MakePalette(Bmp: TBitmap);
  public
    function GenerateHeatMap(const InFileName, OutFileName: PChar;
      NoiseModel: integer; Info: PChar; InfoSize: integer;
      PngFormat: boolean): boolean;
  end;

var
  Gen: THeatMapGenerator;


implementation

{ THeatMapGenerator }

function THeatMapGenerator.GenerateHeatMap(const InFileName,
  OutFileName: PChar; NoiseModel: integer; Info: PChar;
  InfoSize: integer; PngFormat: boolean): boolean;
begin
  FInFileName := InFileName;
  FOutFileName := OutFileName;

  try
    Fields := TStringList.Create;
    try
      AnalyzeFile;
      ImportText;
    finally
      Fields.Free;
    end;

    case NoiseModel of
      2:   SubtractNoise2D;
      1:   SubtractNoise1D;
      else SubtractNoiseConst;
    end;

    MakeImage(PngFormat);

    FInfo := InfoToJson;
    if Length(FInfo) >= InfoSize then raise Exception.Create('File name too long');
    Result := true;
  except on E: Exception do
    begin
    FInfo := E.Message;
    if Length(FInfo) >= InfoSize then SetLength(FInfo, InfoSize-1);
    Result := false;
    end;
  end;

  Move(FInfo[1], Info^, (Length(FInfo)+1) * SizeOf(Char));
end;


//------------------------------------------------------------------------------
//                             import csv
//------------------------------------------------------------------------------
procedure THeatMapGenerator.AnalyzeFile;
var
  S: string;
  Fs: TFileStream;
  Rd: TStreamReader;
begin
  Fs := TFileStream.Create(FInFileName, fmOpenRead or fmShareDenyNone);
  Rd := TStreamReader.Create(Fs);
  S := Rd.ReadLine;
  Fields.CommaText := S;
  TimeStr := Fields[1];
  FreqStart := StrToInt(Fields[2]);
  FreqStep := StrToFloat(Fields[4]);
  TimeStart := Fields[0] + 'T' + Fields[1];

  repeat
    FreqEnd := StrToInt(Fields[3]);
    if  Rd.EndOfStream then Break;
    S := Rd.ReadLine;
    Fields.CommaText := S;
  until Fields[1] <> TimeStr;

  BinCnt := Round((FreqEnd - FreqStart) / FreqStep);
  SetLength(Spect, BinCnt);

  Rd.Free;
  Fs.Free;
end;

procedure THeatMapGenerator.ImportText;
var
  Fs: TFileStream;
  Rd: TStreamReader;
begin
  Fs := TFileStream.Create(FInFileName, fmOpenRead or fmShareDenyNone);
  Rd := TStreamReader.Create(Fs);
  while not Rd.EndOfStream do ProcessLine(Rd.ReadLine);

  //last line
  SetLength(Data, Length(Data) + 1);
  Data[High(Data)] := Spect;

  Rd.Free;
  Fs.Free;
end;

procedure THeatMapGenerator.ProcessLine(S: string);
var
  i, Idx: integer;
begin
  Fields.CommaText := S;

  if (S = '') or (Fields[1] <> TimeStr) then
    begin
    TimeEnd := Fields[0] + 'T' + Fields[1];
    SetLength(Data, Length(Data) + 1);
    Data[High(Data)] := Spect;
    if S = '' then Exit;
    TimeStr := Fields[1];
    Spect := nil;
    SetLength(Spect, BinCnt);
    end;

  Idx := Round((StrToInt(Fields[2]) - FreqStart) / FreqStep);
  for i:=6 to Fields.Count-1 do
    if ((Idx+i-6) >= 0) and ((Idx+i-6) < BinCnt) then
      Spect[Idx+i-6] := StrToFloat(Fields[i]);
end;


//------------------------------------------------------------------------------
//                        subtract noise floor
//------------------------------------------------------------------------------
const
  MAX_BYTE = 255;

procedure THeatMapGenerator.SubtractNoiseConst;
var
  i, j: integer;
  MinV, MaxV: Single;
  MinArr: TSingleArray;
begin
  SetLength(MinArr, BinCnt);
  MinV := Data[0,0];
  MaxV := Data[0,0];

  for i:=0 to High(Data) do
    for j:=0 to High(Data[i]) do
      begin
      MinV := Min(MinV, Data[i,j]);
      MaxV := Max(MaxV, Data[i,j]);
      end;

  for i:=0 to High(Data) do
    for j:=0 to High(Data[i]) do
      Data[i,j] := Max(0, Min(MAX_BYTE, (Data[i,j] - MinV) / (MaxV - MinV) * MAX_BYTE));
end;

procedure THeatMapGenerator.SubtractNoise1D;
var
  i, j: integer;
  MaxV: Single;
  Noise: TSingleArray;
begin
  Integer(Noise) := 0;
  SetLength(Noise, BinCnt);
  MaxV := Data[0,0];

  for i:=0 to High(Data) do
    for j:=0 to High(Data[i]) do
      Noise[j] := Min(Noise[j], Data[i,j]);

  //150 is an empirical filter width that makes the heat map look nice
  Noise := SlidingMin(Noise, 150);
  Noise := SlidingAvg(Noise, 150);

  for i:=0 to High(Data) do
    for j:=0 to High(Data[i]) do
      begin
      Data[i,j] := Max(0, Data[i,j] - Noise[j]);
      MaxV := Max(MaxV, Data[i,j]);
      end;

  for i:=0 to High(Data) do
    for j:=0 to High(Data[i]) do
      Data[i,j] := Max(0, Min(MAX_BYTE, Data[i,j] / MaxV * MAX_BYTE));
end;

procedure THeatMapGenerator.SubtractNoise2D;
var
  i, j: integer;
  MaxV: Single;
  Noise: TSingleArray;
begin

  for i:=0 to High(Data) do
    begin
    Noise := SlidingMin(Data[i], 150);
    Noise := SlidingAvg(Noise, 150);
    for j:=0 to High(Data[i]) do Data[i,j] := Max(0, Data[i,j] - Noise[j]);
    end;

  MaxV := Data[0,0];
  for i:=0 to High(Data) do
    for j:=0 to High(Data[i]) do
      MaxV := Max(MaxV, Data[i,j]);

  for i:=0 to High(Data) do
    for j:=0 to High(Data[i]) do
      Data[i,j] := Data[i,j] / MaxV * MAX_BYTE;
end;


//------------------------------------------------------------------------------
//                             make image
//------------------------------------------------------------------------------
procedure THeatMapGenerator.MakeImage(PngFormat: boolean);
var
  Bmp: TBitmap;
  x, y: integer;
  ScanLine: PByte;
  Png: TPngImage;
begin
  Bmp := TBitmap.Create;
  try
    Bmp.PixelFormat := pf8bit;
    Bmp.SetSize(Length(Data[0]), Length(Data));
    MakePalette(Bmp);

    for y:=0 to High(Data) do
      begin
      ScanLine := Bmp.ScanLine[y];
      for x:=0 to High(Data[y]) do ScanLine[x] := Round(Data[y, x]);
      end;

    Data := nil;

    if PngFormat then
      begin
      Png := TPngImage.Create;
      try
        Png.Assign(Bmp);
        Png.SaveToFile(FOutFileName);
      finally Png.Free; end;
      end
    else
      Bmp.SaveToFile(FOutFileName);
  finally Bmp.Free; end;
end;


procedure InterpolateColors(var APalette: TRgbPalette; IdxB, IdxE: integer);
var
  i: integer;
  dR, dG, dB: Single;
begin
  dR := (APalette[IdxE].rgbRed   - APalette[IdxB].rgbRed)   / (IdxE - IdxB);
  dG := (APalette[IdxE].rgbGreen - APalette[IdxB].rgbGreen) / (IdxE - IdxB);
  dB := (APalette[IdxE].rgbBlue  - APalette[IdxB].rgbBlue)  / (IdxE - IdxB);

  for i:=IdxB+1 to IdxE-1 do
    begin
    APalette[i].rgbRed   := APalette[IdxB].rgbRed   + Round(dR * (i-IdxB));
    APalette[i].rgbGreen := APalette[IdxB].rgbGreen + Round(dG * (i-IdxB));
    APalette[i].rgbBlue  := APalette[IdxB].rgbBlue  + Round(dB * (i-IdxB));
    end;
end;

procedure THeatMapGenerator.MakePalette(Bmp: TBitmap);
const
  Colors: array[0..4] of TRGBQuad = (
    (rgbBlue: $88; rgbGreen: $0;   rgbRed: 0),
    (rgbBlue: $ff; rgbGreen: $ff;  rgbRed:$0),
    (rgbBlue: $0;  rgbGreen: $FF;  rgbRed: $ff),
    (rgbBlue: 0;   rgbGreen: $0;   rgbRed: $ff),
    (rgbBlue: $Ff; rgbGreen: $FF;  rgbRed: $FF));
var
  i, i1, i2: integer;
  Step: Single;
  ColorTable: TRgbPalette;
begin
  FillChar(ColorTable, SizeOf(ColorTable), #0);
  Step := MAX_BYTE / (Length(Colors) - 1);
  ColorTable[0] := Colors[0];
  for i:=1 to High(Colors) do
    begin
    i1 := Round((i-1) * Step);
    i2 := Round(i * Step);
    ColorTable[i2] := Colors[i];
    InterpolateColors(ColorTable, i1, i2);
    end;

  Bmp.Palette := 0;
  SetDibColorTable(Bmp.Canvas.Handle, 0, MAX_BYTE+1, ColorTable[0]);
end;



function THeatMapGenerator.InfoToJson: string;
const
  Fmt = '{"Name":"%s","StartFreq":%d,"EndFreq":%d,' +
        '"StartTime":"\/Date(%d)\/","EndTime":"\/Date(%d)\/"}';
var
  TimeFmt: TFormatSettings;
  DateB, DateE: TDateTime;
  NetDateB, NetDateE: Int64;
  FileName: TFileName;
begin
  //date format in csv file
  GetLocaleFormatSettings(GetThreadLocale, TimeFmt);
  TimeFmt.DateSeparator := '-';
  TimeFmt.ShortDateFormat := 'yyyy-mm-dd';
  TimeFmt.TimeSeparator := ':';
  TimeFmt.LongTimeFormat := 'hh:nn:ss';

  //string to delphi date
  DateB := StrToDateTime(TimeStart, TimeFmt);
  if TimeEnd = ''
    then DateE := DateB + 1/86400
    else DateE := StrToDateTime(TimeEnd, TimeFmt);

  //delphi date to .net date
  NetDateB := Round((DateB - EncodeDate(1970,1,1)) * 86400000);
  NetDateE := Round((DateE - EncodeDate(1970,1,1)) * 86400000);

  FileName := ChangeFileExt(ExtractFileName(FOutFileName), '');

  Result := Format(Fmt, [FileName, FreqStart, FreqEnd, NetDateB, NetDateE]);
end;

initialization
  Gen := THeatMapGenerator.Create;

finalization
  Gen.Free;

end.
