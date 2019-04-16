"%wix%\bin\candle" drivers.wxs -ext WiXNetFxExtension -ext WixDifxAppExtension -ext WixUIExtension.dll -ext WixUtilExtension -ext WixIisExtension
pause
"%wix%\bin\light" drivers.wixobj "%wix%\bin\difxapp_x86.wixlib" -o driver.msi -ext WiXNetFxExtension -ext WixDifxAppExtension -ext WixUIExtension.dll -ext WixUtilExtension -ext WixIisExtension
pause