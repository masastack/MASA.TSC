FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY ./publish .
COPY ["src/Web/Masa.Tsc.Web.Admin.Rcl/wwwroot/i18n","wwwroot/i18n"]
ENV ASPNETCORE_URLS=http://0.0.0.0:80
ENTRYPOINT ["dotnet", "Masa.Tsc.Admin.Server.dll"]