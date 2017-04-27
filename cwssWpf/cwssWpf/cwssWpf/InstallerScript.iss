[Setup]
AppName=CWSS System
AppVersion=1.0
DefaultDirName={pf}\CWSS System
DefaultGroupName=CWSS System
UninstallDisplayIcon={app}\cwssWpf.exe
Compression=lzma2
SolidCompression=yes
OutputDir=C:\Users\Derek\Source\Repos\CWSSv2\cwssWpf\cwssWpf\cwssWpf\SetupExe
OutputBaseFilename=CWSS Setup
PrivilegesRequired=admin

[Components]
Name: client; Description: Install for a remote client machine.; Flags: Dontinheritcheck
Name: msr; Description: Install magnetic strip reader config app.; Flags: Dontinheritcheck

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; \
    GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "bin\Release\cwssWpf.exe"; DestDir: "{app}"
Source: "bin\Release\ImageProcessor.dll"; DestDir: "{app}"
Source: "bin\Release\itextsharp.dll"; DestDir: "{app}"
Source: "bin\Release\Newtonsoft.Json.dll"; DestDir: "{app}"
Source: "bin\Release\RijndaelEncryptDecrypt.dll"; DestDir: "{app}"
Source: "bin\Release\WebEye.Controls.Wpf.WebCameraControl.dll"; DestDir: "{app}"
Source: "bin\Release\UserManual.txt"; DestDir: "{app}"; Flags: isreadme

Source: "bin\Release\AppData\ClientSetup.cfg"; DestDir: "{app}\AppData"; Components: client

Source: "bin\Release\AppData\MSR Config\msr90Config_v220.exe"; DestDir: "{app}\MSR Config"; Components: msr
Source: "bin\Release\AppData\MSR Config\uicomm.dll"; DestDir: "{app}\MSR Config"; Components: msr

Source: "bin\Release\Sounds\CheckIn.wav"; DestDir: "{app}\Sounds"
Source: "bin\Release\Sounds\CheckOut.wav"; DestDir: "{app}\Sounds"
Source: "bin\Release\Sounds\Fail.wav"; DestDir: "{app}\Sounds"
Source: "bin\Release\Sounds\LogIn.wav"; DestDir: "{app}\Sounds"
Source: "bin\Release\Sounds\LogOff.wav"; DestDir: "{app}\Sounds"
Source: "bin\Release\Sounds\Success.wav"; DestDir: "{app}\Sounds"

Source: "bin\Release\Images\Connect.png"; DestDir: "{app}\Images"
Source: "bin\Release\Images\Disconnect.png"; DestDir: "{app}\Images"
Source: "bin\Release\Images\Alert.png"; DestDir: "{app}\Images"
Source: "bin\Release\Images\Success.png"; DestDir: "{app}\Images"
Source: "bin\Release\Images\Waiver.png"; DestDir: "{app}\Images"

Source: "bin\Release\AppData\CW Acknowledgement of Risk and Sign-In Sheet.pdf"; DestDir: "{app}\AppData"

[Icons]
Name: "{group}\CWSS System"; Filename: "{app}\cwssWpf.exe"

[Run]
Filename: "{app}\cwssWpf.exe"; Description: "{cm:LaunchProgram,CWSS System}"; Flags: runascurrentuser nowait postinstall skipifsilent
