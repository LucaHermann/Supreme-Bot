@echo off
"C:\Program Files (x86)\Microsoft\ILMerge\ilmerge.exe" /ndebug /copyattrs /targetplatform:4.0,"C:\Windows\Microsoft.NET\Framework64\v4.0.30319" /out:"bin\Release\BOT Supreme.exe" "bin\Release\bot_supreme.exe" "bin\Release\HwidSystem.dll" "bin\Release\HtmlAgilityPack.dll"
Del "bin\Release\HwidSystem.dll"
Del "bin\Release\HtmlAgilityPack.dll"
Del "bin\Release\*.pdb"
Del "bin\Release\*.xml"
Del "bin\Release\bot_supreme.exe"
Del "bin\Release\bot_supreme.exe.config"
echo Merged
pause