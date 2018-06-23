unit SlidingFilters;

interface

uses
  SysUtils;

type
  TSingleArray = array of Single;

function SlidingMin(Arr: TSingleArray; Wid: integer): TSingleArray;
function SlidingAvg(Arr: TSingleArray; Wid: integer): TSingleArray;


implementation

function SlidingMin(Arr: TSingleArray; Wid: integer): TSingleArray;
var
  Len, i, j, IModLen: integer;
  MinAhead: TSingleArray;
  MinG, MinR: Single;
begin
  Len := 2*Wid + 1;
  SetLength(Result, Length(Arr));
  SetLength(MinAhead, Length(Arr));

  //first block: set Result[i] to Min(Arr[0]..Arr[i+Wid])

  MinG := Arr[0];
  for i:=1 to Wid-1 do
    if Arr[i] < MinG then MinG := Arr[i];

  for i:=Wid to Len-1 do
    begin
    if Arr[i] < MinG then MinG := Arr[i];
    Result[i-Wid] := MinG;
    end;

  //slide window one sample at a time
  //set the rest of Result[]

  IModLen := Len-1;
  for i:=Len to High(Arr) do
    begin
    //Slide;
    if Arr[i] < MinG then MinG := Arr[i];

    Inc(IModLen);
    if IModLen = Len then
      begin
      IModLen := 0;
      MinG := Arr[i];
      MinR := Arr[i];
      for j:=i downto i-Len do
        begin
        MinAhead[j] := MinR;
        if Arr[j] < MinR then MinR := Arr[j];
        end;
      end;

    MinR := MinAhead[i-Len];

    if MinG < MinR
      then Result[i-Wid] := MinG
      else Result[i-Wid] := MinR;
    end;

  //last block: set Result[i] to Min(Arr[i-Wid]..Arr[High(Arr)])

  MinG := Arr[High(Arr)];
  for i:=High(Arr)-1 downto High(Arr)-Wid do
    if Arr[i] < MinG then MinG := Arr[i];

  for i:=High(Result) downto High(Result)-Wid+1 do
    begin
    if Arr[i-Wid] < MinG then MinG := Arr[i-Wid];
    Result[i] := MinG;
    end;
end;

function SlidingAvg(Arr: TSingleArray; Wid: integer): TSingleArray;
var
  i: integer;
  V, Scale: Single;
begin
  SetLength(Result, Length(Arr));
  Scale := 1 / (2*Wid+1);

  V := 0;
  for i:=0 to 2*Wid do
    begin
    V := V + Arr[i];
    Result[i div 2] := V / (i+1);
    end;

  for i:=Wid to High(Arr)-Wid-1 do
    begin
    Result[i] := V * Scale;
    V := V - Arr[i-Wid] + Arr[i+Wid+1];
    end;

  V := 0;
  for i:=0 to 2*Wid do
    begin
    V := V + Arr[High(Arr)-i];
    Result[High(Arr) - (i div 2)] := V / (i+1);
    end;
end;

end.
