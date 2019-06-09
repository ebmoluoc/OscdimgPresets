[Setup]
AppId={{FDCE8743-7F06-483A-9FBA-A5457E23B507}
SetupMutex=Global\FDCE8743-7F06-483A-9FBA-A5457E23B507
AppMutex=Global\CC8880E4-B4EA-48FC-B9D6-AE83B3442C0E
AppCopyright=Copyright (c) 2019 Philippe Coulombe
AppPublisher=Philippe Coulombe
AppVersion=2.3.0.0
VersionInfoVersion=2.3.0.0
AppVerName=Oscdimg Presets 2.3
AppName=Oscdimg Presets
DefaultDirName={commonpf}\Oscdimg Presets
UninstallDisplayIcon={app}\OscdimgPresets.exe
OutputBaseFilename=OscdimgPresetsSetup
OutputDir=.
LicenseFile=LICENSE
DisableProgramGroupPage=yes
DisableDirPage=yes
SolidCompression=yes
ArchitecturesAllowed=x64
ArchitecturesInstallIn64BitMode=x64
MinVersion=6.1.7601
WizardSizePercent=120,100

[Files]
Source: "LICENSE"; DestDir: {app}; Flags: restartreplace uninsrestartdelete ignoreversion
;Source: "Oscdimg\x64\oscdimg.exe"; DestDir: {app}; Flags: restartreplace uninsrestartdelete ignoreversion
Source: "OscdimgPresets\bin\x64\Release\CsharpHelpers.dll"; DestDir: {app}; Flags: restartreplace uninsrestartdelete ignoreversion
Source: "OscdimgPresets\bin\x64\Release\OscdimgPresets.exe"; DestDir: {app}; Flags: restartreplace uninsrestartdelete ignoreversion
Source: "OscdimgPresets\bin\x64\Release\OscdimgPresets.exe.config"; DestDir: {app}; Flags: restartreplace uninsrestartdelete ignoreversion
Source: "x64\Release\OscdimgPresetsExt.dll"; DestDir: {app}; Flags: restartreplace uninsrestartdelete ignoreversion regserver

[Icons]
Name: "{commonprograms}\Oscdimg Presets"; Filename: "{app}\OscdimgPresets.exe"
Name: "{commondesktop}\Oscdimg Presets"; Filename: "{app}\OscdimgPresets.exe"; Tasks: desktopicon

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; Flags: unchecked

[Run]
Filename: "{app}\OscdimgPresets.exe"; Description: "{cm:LaunchProgram,Oscdimg Presets}"; Flags: nowait postinstall skipifsilent unchecked

[Code]
procedure InitializeWizard();
begin
    WizardForm.LicenseMemo.Font.Name := 'Consolas';
end;
