FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

COPY ProductCatalog.API/*.csproj ./ProductCatalog.API/
COPY ProductCatalog.Application/*.csproj ./ProductCatalog.Application/
COPY ProductCatalog.Domain/*.csproj ./ProductCatalog.Domain/
COPY ProductCatalog.Infrastructure/*.csproj ./ProductCatalog.Infrastructure/

RUN dotnet restore ProductCatalog.API/ProductCatalog.API.csproj

COPY . ./

RUN dotnet publish ProductCatalog.API/ProductCatalog.API.csproj -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/out .

EXPOSE 80
EXPOSE 443

ENV LANG pt_BR.UTF-8
ENV ASPNETCORE_URLS=http://+:80

CMD ["dotnet", "ProductCatalog.API.dll"]