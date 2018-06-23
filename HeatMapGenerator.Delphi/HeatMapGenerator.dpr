library HeatMapGenerator;
uses
  SysUtils,
  HeatMapGen in 'HeatMapGen.pas',
  SlidingFilters in 'SlidingFilters.pas';

{$R *.res}

function GenerateHeatMap(const InFileName, OutFileName: PChar;
  NoiseModel: integer; Info: PChar; InfoSize: integer;
  PngFormat: boolean): boolean; stdcall;
begin
  Result := Gen.GenerateHeatMap(InFileName, OutFileName, NoiseModel, Info,
    InfoSize, PngFormat);
end;

exports GenerateHeatMap;

begin
end.
