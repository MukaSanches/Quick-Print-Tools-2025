Unicode true
Name "Quick Print Studio"
OutFile "QuickPrintStudio-Setup.exe"
RequestExecutionLevel admin
InstallDir "$PROGRAMFILES64\\Quick Print Studio"
ShowInstDetails show
ShowUninstDetails show
!include "LogicLib.nsh"
!include "x64.nsh"
Var CorelRoot
Var AddonDir
Function DetectCorel
  StrCpy $CorelRoot ""
  ${If} ${RunningX64}
    SetRegView 64
  ${EndIf}
  ReadRegStr $CorelRoot HKLM "SOFTWARE\\Corel\\CorelDRAW\\26.0" "InstallDir"
  ${If} $CorelRoot == ""
    StrCpy $0 "$PROGRAMFILES64\\Corel\\CorelDRAW Graphics Suite 2025\\Programs64"
    IfFileExists "$0\\CorelDRW.exe" 0 +2
      StrCpy $CorelRoot "$0"
  ${EndIf}
  ${If} $CorelRoot == ""
    MessageBox MB_ICONSTOP "CorelDRAW 2025 não foi encontrado automaticamente. A instalação foi cancelada para evitar copiar arquivos no local errado."
    Abort
  ${EndIf}
  IfFileExists "$CorelRoot\\CorelDRW.exe" +2 0
    MessageBox MB_ICONSTOP "A instalação detectada não contém CorelDRW.exe. Instalação cancelada."
    Abort
  StrCpy $AddonDir "$CorelRoot\\Addons\\QuickPrintStudio"
FunctionEnd
Section "Quick Print Studio" SEC01
  Call DetectCorel
  SetOutPath "$AddonDir"
  File /r "..\\dist\\addon\\*.*"
  WriteUninstaller "$INSTDIR\\Uninstall.exe"
  WriteRegStr HKLM "Software\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\QuickPrintStudio" "DisplayName" "Quick Print Studio"
  WriteRegStr HKLM "Software\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\QuickPrintStudio" "UninstallString" '\"$INSTDIR\\Uninstall.exe\"'
SectionEnd
Section "Uninstall"
  RMDir /r "$AddonDir"
  Delete "$INSTDIR\\Uninstall.exe"
  RMDir "$INSTDIR"
  DeleteRegKey HKLM "Software\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\QuickPrintStudio"
SectionEnd
