
$env:PATH="$env:PATH;C:\Program Files\7-Zip;C:\Program Files (x86)\Android\android-sdk\build-tools\29.0.2;C:\Program Files\Android\Android Studio\jre\bin;C:\Program Files (x86)\Microsoft Visual Studio\2019\Preview\MSBuild\Current\Bin;C:\Program Files (x86)\Microsoft Visual Studio\Preview\Community\MSBuild\15.0\Bin;C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin"

#prep 
$current = Get-Location
          
$days = [int]((New-TimeSpan -Start 2020-01-01 -End (Get-Date)).totaldays * 100)
          
$manifest = [xml](Get-Content -Path ExtLibs\Xamarin\Xamarin.Android\Properties\AndroidManifest.xml -Raw)

$manifest.SelectNodes("manifest") | % { $_.versionCode = ""+$days }
$manifest.SelectNodes("manifest") | % { $_.versionName = ""+$days }
          
$manifest.Save($current.Path + "\ExtLibs\Xamarin\Xamarin.Android\Properties\AndroidManifest.xml")


# build it
msbuild -v:m -restore -t:SignAndroidPackage -p:Configuration=Release "ExtLibs\Xamarin\Xamarin.Android\Xamarin.Android.csproj"

#package it
#rem zip -d my_application.apk META-INF/\*
#rem 7z d ExtLibs\Xamarin\Xamarin.Android\bin\Release\com.michaeloborne.MissionPlanner.apk META-INF/\*
#rem keytool -genkey -v -keystore my-release-key.keystore -alias key -keyalg RSA -keysize 2048 -validity 10000

#rem keytool -v -list -keystore %USERPROFILE%\key.keystore

del ExtLibs\Xamarin\Xamarin.Android\bin\Release\com.michaeloborne.MissionPlanner-Signed.aab

jarsigner -verbose -sigalg SHA256withRSA -digestalg SHA-256 -keystore $env:USERPROFILE\key.keystore ExtLibs\Xamarin\Xamarin.Android\bin\Release\com.michaeloborne.MissionPlanner.aab key
zipalign -v 4 ExtLibs\Xamarin\Xamarin.Android\bin\Release\com.michaeloborne.MissionPlanner.aab ExtLibs\Xamarin\Xamarin.Android\bin\Release\com.michaeloborne.MissionPlanner-Signed.aab

pause
