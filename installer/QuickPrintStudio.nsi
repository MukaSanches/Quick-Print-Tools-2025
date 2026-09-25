Unicode true
!define PRODUCT "Quick Print Studio"
!define VERSION "1.0.0"
Name "${PRODUCT} ${VERSION}"
OutFile "QuickPrintStudio-1.0.0-Setup.exe"
RequestExecutionLevel admin
InstallDir "$PROGRAMFILES64\Quick Print Studio"
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
  ReadRegStr $CorelRoot HKLM "SOFTWARE\Corel\CorelDRAW\26.0" "InstallDir"
  ${If} $CorelRoot == ""
    StrCpy $0 "$PROGRAMFILES64\Corel\CorelDRAW Graphics Suite 2025\Programs64"
    IfFileExists "$0\CorelDRW.exe" 0 +2
      StrCpy $CorelRoot "$0"
  ${EndIf}
  ${If} $CorelRoot == ""
    MessageBox MB_ICONSTOP "CorelDRAW 2025 64-bit nao foi encontrado. Nada foi alterado."
    Abort
  ${EndIf}
  IfFileExists "$CorelRoot\CorelDRW.exe" +2 0
    MessageBox MB_ICONSTOP "A pasta detectada nao contem CorelDRW.exe. Instalacao cancelada."
    Abort
  StrCpy $AddonDir "$CorelRoot\Addons\QuickPrintStudio"
FunctionEnd

Section "Quick Print Studio" SEC01
  Call DetectCorel
  SetOutPath "$AddonDir"
  File /r "..\dist\addon\*.*"
  SetOutPath "$INSTDIR"
  WriteUninstaller "$INSTDIR\Uninstall.exe"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\QuickPrintStudio" "DisplayName" "${PRODUCT}"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\QuickPrintStudio" "DisplayVersion" "${VERSION}"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\QuickPrintStudio" "Publisher" "MukaSanches"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\QuickPrintStudio" "UninstallString" '"$INSTDIR\Uninstall.exe"'
SectionEnd

Section "Uninstall"
  SetRegView 64
  ReadRegStr $CorelRoot HKLM "SOFTWARE\Corel\CorelDRAW\26.0" "InstallDir"
  ${If} $CorelRoot == ""
    StrCpy $CorelRoot "$PROGRAMFILES64\Corel\CorelDRAW Graphics Suite 2025\Programs64"
  ${EndIf}
  StrCpy $AddonDir "$CorelRoot\Addons\QuickPrintStudio"
  RMDir /r "$AddonDir"
  Delete "$INSTDIR\Uninstall.exe"
  RMDir "$INSTDIR"
  DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\QuickPrintStudio"
SectionEnd
