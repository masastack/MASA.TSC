FROM registry.cn-hangzhou.aliyuncs.com/masa/dotnet_sdk:6.0.100 AS publish
WORKDIR /src
COPY . .
ENV CSPROJ="src/Web/Masa.Tsc.Admin.Server/Masa.Tsc.Admin.Server.csproj"
RUN dotnet restore $CSPROJ && dotnet publish $CSPROJ -c Release -o /app/publish

FROM registry.cn-hangzhou.aliyuncs.com/masa/dotnet_aspnet:6.0.0
WORKDIR /app
COPY --from=publish /app/publish .
COPY ["src/Web/Masa.Tsc.Web.Admin.Rcl/wwwroot/i18n","wwwroot/i18n"]
ENV ASPNETCORE_URLS=https://localhost:18012;http://localhost:18013
EXPOSE 18012
EXPOSE 18013
ENTRYPOINT ["dotnet", "Masa.Tsc.Admin.Server.dll"]