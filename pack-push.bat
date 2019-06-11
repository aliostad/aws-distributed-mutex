del /F /Q .\artifacts\*.*
dotnet pack AwsDistributedMutex.sln -o ..\..\artifacts
dotnet nuget push "artifacts\*.nupkg" -s nuget.org