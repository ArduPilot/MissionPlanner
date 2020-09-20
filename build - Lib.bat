
set PATH=%PATH%;C:\Program Files (x86)\Android\android-sdk\build-tools\29.0.2;C:\Program Files\Android\Android Studio\jre\bin;C:\Program Files (x86)\Microsoft Visual Studio\2019\Preview\MSBuild\Current\Bin;C:\Program Files (x86)\Microsoft Visual Studio\Preview\Community\MSBuild\15.0\Bin;C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin

msbuild -v:m -restore -t:SignAndroidPackage -p:Configuration=Release "ExtLibs\Xamarin\Xamarin.Android\Xamarin.Android.csproj"

rem zip -d my_application.apk META-INF/\*
rem keytool -genkey -v -keystore my-release-key.keystore -alias key -keyalg RSA -keysize 2048 -validity 10000

rem keytool -v -list -keystore %USERPROFILE%\key.keystore

del ExtLibs\Xamarin\Xamarin.Android\bin\Release\com.michaeloborne.MissionPlanner-signed2.apk

jarsigner -verbose -sigalg SHA256withRSA -digestalg SHA-256 -keystore %USERPROFILE%\key.keystore ExtLibs\Xamarin\Xamarin.Android\bin\Release\com.michaeloborne.MissionPlanner.apk key
zipalign -v 4 ExtLibs\Xamarin\Xamarin.Android\bin\Release\com.michaeloborne.MissionPlanner.apk ExtLibs\Xamarin\Xamarin.Android\bin\Release\com.michaeloborne.MissionPlanner-signed2.apk

pause
