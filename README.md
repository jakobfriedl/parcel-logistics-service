# Friedl-Haider Parcel Logistics Service

Semesterproject of the course "Software Components Systems Lab" at UAS Technikum Wien. Implemented by Jakob Friedl (if20b089) and Philipp Haider (if20b097).

## Upgrade NuGet Packages

NuGet packages get frequently updated.

To upgrade this solution to the latest version of all NuGet packages, use the dotnet-outdated tool.


Install dotnet-outdated tool:

```
dotnet tool install --global dotnet-outdated-tool
```

Upgrade only to new minor versions of packages

```
dotnet outdated --upgrade --version-lock Major
```

Upgrade to all new versions of packages (more likely to include breaking API changes)

```
dotnet outdated --upgrade
```


## Run

Linux/OS X:

```
sh build.sh
```

Windows:

```
build.bat
```
## Run in Docker

```
cd src/FH.ParcelLogistics.Services
docker build -t fh.parcellogistics.services .
docker run -p 5000:8080 fh.parcellogistics.services
```
