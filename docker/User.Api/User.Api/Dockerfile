FROM microsoft/aspnetcore-build as build-env
WORKDIR /code
COPY *.csproj ./
run dotnet restore
COPY . ./
RUN dotnet publish -c Release -o out

FROM microsoft/aspnetcore
WORKDIR /app
COPY --from=build-env /code/out ./

EXPOSE 5000
ENTRYPOINT ["dotnet", "User.Api.dll"]