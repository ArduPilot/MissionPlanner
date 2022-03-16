
adb shell am force-stop com.michaeloborne.MissionPlanner

adb shell setprop debug.mono.profile log:sample
adb shell run-as com.michaeloborne.MissionPlanner ls -l files/.__override__

adb shell am broadcast -a "mono.android.intent.action.EXTERNAL_STORAGE_DIRECTORY" -n "Mono.Android.DebugRuntime/com.xamarin.mono.android.ExternalStorageDirectory"
adb shell am start -a "android.intent.action.MAIN" -c "android.intent.category.LAUNCHER" -n "com.michaeloborne.MissionPlanner/crc64d01531a0ef5679fa.MainActivity"

echo press after finished run
pause
adb root
adb pull /data/data/com.michaeloborne.MissionPlanner/files/.__override__

adb shell setprop debug.mono.profile \"\"

pause
