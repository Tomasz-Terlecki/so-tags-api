FROM mcr.microsoft.com/dotnet/sdk:10.0

WORKDIR /src

COPY ["SoTags/SoTags.Tests.Integration/SoTags.Tests.Integration.csproj", "SoTags/SoTags.Tests.Integration/"]
COPY ["SoTags/SoTags.Domain/SoTags.Domain.csproj", "SoTags/SoTags.Domain/"]
COPY ["SoTags/SoTags.Repo/SoTags.Repo.csproj", "SoTags/SoTags.Repo/"]
COPY ["SoTags/SoTags.API/SoTags.API.csproj", "SoTags/SoTags.API/"]
COPY ["SoTags/SoTags.DataProvider/SoTags.DataProvider.csproj", "SoTags/SoTags.DataProvider/"]

RUN dotnet restore "SoTags/SoTags.Tests.Integration/SoTags.Tests.Integration.csproj"

COPY ./SoTags ./SoTags

WORKDIR /src/SoTags/SoTags.Tests.Integration

ENTRYPOINT ["dotnet", "test", "--configuration", "Release", "--logger", "trx;LogFileName=test_results.trx"]

