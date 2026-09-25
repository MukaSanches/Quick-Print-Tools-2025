Unicode true
!define PRODUCT "Quick Print Studio"
!define VERSION "1.2.0"
Name "${PRODUCT} ${VERSION}"
OutFile "QuickPrintStudio-1.2.0-Setup.exe"
RequestExecutionLevel admin
InstallDir "$PROGRAMFILES64\Quick Print Studio"
ShowInstDetails show
ShowUninstDetails show
!include "LogicLib.nsh"
!include "x64.nsh"
!include "MUI2.nsh"

Var CorelRoot
Var AddonDir
Var VGCorePath

!define MUI_ABORTWARNING
!define MUI_DIRECTORYPAGE_TEXT_TOP "CorelDRAW detectado automaticamente quando possivel. Confirme a pasta abaixo. Se estiver incorreta, selecione a pasta que contem CorelDRW.exe (normalmente Programs64)."
!define MUI_DIRECTORYPAGE_VARIABLE $CorelRoot
!insertmacro MUI_PAGE_WELCOME
!insertmacro MUI_PAGE_DIRECTORY
!insertmacro MUI_PAGE_INSTFILES
!insertmacro MUI_PAGE_FINISH
!insertmacro MUI_LANGUAGE "PortugueseBR"

Function .onInit
  StrCpy $CorelRoot ""
  ${If} ${RunningX64}
    SetRegView 64
  ${EndIf}

  ; 1) Prefer the official CorelDRAW 2025 registry location.
  ReadRegStr $CorelRoot HKLM "SOFTWARE\Corel\CorelDRAW\26.0" "InstallDir"
  IfFileExists "$CorelRoot\CorelDRW.exe" found 0
  StrCpy $CorelRoot ""

  ; 2) Some installations register the suite root instead of Programs64.
  ReadRegStr $0 HKLM "SOFTWARE\Corel\CorelDRAW Graphics Suite\26.0" "InstallDir"
  IfFileExists "$0\Programs64\CorelDRW.exe" 0 +3
    StrCpy $CorelRoot "$0\Programs64"
    Goto found
  IfFileExists "$0\CorelDRW.exe" 0 +3
    StrCpy $CorelRoot "$0"
    Goto found

  ; 3) Validate known 64-bit default locations.
  StrCpy $0 "$PROGRAMFILES64\Corel\CorelDRAW Graphics Suite 2025\Programs64"
  IfFileExists "$0\CorelDRW.exe" 0 +3
    StrCpy $CorelRoot "$0"
    Goto found
  StrCpy $0 "$PROGRAMFILES64\Corel\CorelDRAW Graphics Suite\Programs64"
  IfFileExists "$0\CorelDRW.exe" 0 +3
    StrCpy $CorelRoot "$0"
    Goto found

  ; 4) Nothing reliable found: give the user a useful starting folder.
  StrCpy $CorelRoot "$PROGRAMFILES64\Corel"
  MessageBox MB_ICONINFORMATION "O Quick Print Studio nao encontrou automaticamente uma instalacao valida do CorelDRAW 2025. Na proxima tela, selecione a pasta que contem CorelDRW.exe (normalmente Programs64)."
  Goto done

found:
  DetailPrint "CorelDRAW encontrado automaticamente: $CorelRoot"
done:
FunctionEnd

Function ValidateCorelFolder
  IfFileExists "$CorelRoot\CorelDRW.exe" corel_ok 0
  MessageBox MB_ICONSTOP "Essa pasta nao parece ser a pasta do CorelDRAW. Selecione a pasta que contem CorelDRW.exe."
  Abort
corel_ok:
  StrCpy $VGCorePath ""
  IfFileExists "$CorelRoot\Assemblies\Corel.Interop.VGCore.dll" 0 +3
    StrCpy $VGCorePath "$CorelRoot\Assemblies\Corel.Interop.VGCore.dll"
    Goto vg_done
  IfFileExists "$CorelRoot\Corel.Interop.VGCore.dll" 0 +3
    StrCpy $VGCorePath "$CorelRoot\Corel.Interop.VGCore.dll"
    Goto vg_done
  ; A versao atual nao redistribui a DLL proprietaria. Registra ausencia para diagnostico,
  ; mas o instalador do painel pode continuar porque o binario nao possui referencia de build a VGCore.
  DetailPrint "Aviso: Corel.Interop.VGCore.dll nao encontrada nesta pasta."
vg_done:
  StrCpy $AddonDir "$CorelRoot\Addons\QuickPrintStudio"
FunctionEnd

Section "Quick Print Studio" SEC01
  Call ValidateCorelFolder
  SetOutPath "$AddonDir"
  File /r "..\dist\addon\*.*"

  SetOutPath "$INSTDIR"
  FileOpen $0 "$INSTDIR\CorelIntegration.txt" w
  FileWrite $0 "CorelDRAW=$CorelRoot$\r$\n"
  FileWrite $0 "VGCore=$VGCorePath$\r$\n"
  FileClose $0
  WriteUninstaller "$INSTDIR\Uninstall.exe"

  SetRegView 64
  WriteRegStr HKLM "Software\QuickPrintStudio" "CorelRoot" "$CorelRoot"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\QuickPrintStudio" "DisplayName" "${PRODUCT}"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\QuickPrintStudio" "DisplayVersion" "${VERSION}"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\QuickPrintStudio" "Publisher" "MukaSanches"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\QuickPrintStudio" "UninstallString" '"$INSTDIR\Uninstall.exe"'
SectionEnd

Section "Uninstall"
  SetRegView 64
  ReadRegStr $CorelRoot HKLM "Software\QuickPrintStudio" "CorelRoot"
  ${If} $CorelRoot != ""
    StrCpy $AddonDir "$CorelRoot\Addons\QuickPrintStudio"
    RMDir /r "$AddonDir"
  ${EndIf}
  Delete "$INSTDIR\CorelIntegration.txt"
  Delete "$INSTDIR\Uninstall.exe"
  RMDir "$INSTDIR"
  DeleteRegKey HKLM "Software\QuickPrintStudio"
  DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\QuickPrintStudio"
SectionEnd
