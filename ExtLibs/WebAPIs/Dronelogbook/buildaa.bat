rem python3 -c "from urllib.request import urlretrieve; urlretrieve('https://oss.sonatype.org/content/repositories/releases/io/swagger/swagger-codegen-cli/2.4.19/swagger-codegen-cli-2.4.19.jar', 'swagger-codegen-cli-2.4.19.jar')"

rem python3 -c "from urllib.request import urlretrieve; urlretrieve('https://nuget.org/nuget.exe', 'nuget.exe')"

java -jar swagger-codegen-cli-2.2.1.jar generate -i swagger.json -l csharp -c config.json

wsl bash build.sh