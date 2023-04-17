FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env

ARG buildVersion

WORKDIR /app
COPY . ./

RUN dotnet restore ./Test.sln

RUN dotnet test ./Test.sln /p:CollectCoverage=true /p:Exclude=\"[*Dapper*]*,[*API*]*,[*Core*]*,[*Proto*]*,[*Service*]Indra.Protos.*\" /p:Threshold=90 /p:ThresholdType=\"line,method\"

RUN dotnet publish ./Test/ -c Release  -o out

FROM mcr.microsoft.com/dotnet/aspnet:7.0

WORKDIR /app
COPY --from=build-env /app/out .

ENV TZ=Asia/Tehran
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone
RUN dpkg-reconfigure -f noninteractive tzdata

EXPOSE 80 80/tcp

ENTRYPOINT ["dotnet", "Test.dll"]