#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM registry.cn-hangzhou.aliyuncs.com/masa/dotnet_sdk:6.0.403 AS publish
WORKDIR /src
COPY . .
ENV CSPROJ="./src/Web/Masa.Tsc.Web.Admin.Server/Masa.Tsc.Web.Admin.Server.csproj"
RUN dotnet restore $CSPROJ -s https://repo.huaweicloud.com/repository/nuget/v3/index.json && dotnet publish $CSPROJ -c Release -o /app/publish

FROM registry.cn-hangzhou.aliyuncs.com/masa/dotnet_aspnet:6.0.4
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=https://0.0.0.0:443
EXPOSE 443

ENTRYPOINT ["dotnet", "Masa.Tsc.Web.Admin.Server.dll"]