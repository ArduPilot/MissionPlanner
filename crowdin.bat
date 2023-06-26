
"c:\Program Files\7-Zip\7z.exe" a -r -x!*.id-ID.resx -x!*.tr.resx -x!*.ru-KZ.resx -x!*.pt.resx -x!*.ko-KR.resx -x!*.ja-JP.resx -x!*.zh-TW.resx -x!*.zh-Hant.resx -x!*.zh-Hans.resx -x!*.az-Latn-AZ.resx -x!*.pl.resx -x!*.it-IT.resx -x!*.fr.resx -x!*.es-ES.resx -x!*.de-DE.resx -x!*.ar.resx -x!*.ru-RU.resx -xr!mono -xr!zedgraph   crowdin.zip *.resx 

"c:\Program Files\7-Zip\7z.exe" a -r -xr!mono -xr!zedgraph   crowdin-all.zip *.resx 
