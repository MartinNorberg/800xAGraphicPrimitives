# Deploying a Graphic Primitive
Making a created DLL useable two steps are required:
## Installing the DLL file
Just copy the DLL file to the 800xA GraphicPrimitives folder. This is usually 
```
c:\Program Files (x86)\ABB 800xA\Base\bin\GraphicPrimitives\
``` on 6.x or 
```
c:\Program Files (x86)\ABB Industrial IT\Operate IT\Process Portal A\bin\GraphicPrimitives\
``` on 5.1.

The PG2 system checks all DLL files in this folder on first startup. So if you have added a new dll you have to close all Plant Explorer and Graphic Builder instances on this node once before it appears in the Graphic Builder.

To delete or overwrite the DLL file, keep in mind the file is write protected while it is still used by running Plant Explorer or Graphics Builder. So you have to close all instances of both on this node before.

## Solve the signing issue.
Graphic Primitives are digital signed, and the PG2 system does not load dll's who are unsigned or signed with the wrong key.
Since we do not have the signing key, we are forced to use a dirty trick. We add the correct public signing key without really signing the DLL.
The PG2 system does not check the correct signing itself, it only check the public key. But now Windows refuses to load the DLL because it is not correct signed.
Lucky windows let us bypass this issue by registering our DLL as an exception.

Another job for the sn-tool:
- Install or locate the **sn.exe**. The sn.exe is part of the Microsoft .Net Framework Tools, see [here](https://docs.microsoft.com/en-us/previous-versions/dotnet/netframework-2.0/k5b5tt23(v=vs.80)?redirectedfrom=MSDN)
- Run a cmd shell as Administrator, cd to the sn.exe (the SDK is not in PATH environment), run sn with the -Vr switch.
```
Microsoft Windows [Version 6.3.9600]
(c) 2013 Microsoft Corporation. All rights reserved.

C:\Windows\system32>cd "\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.8 Tools"

C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.8 Tools>sn -Vr "c:\Program Files (x86)\ABB 800xA\Base\bin\GraphicPrimitives\ElementTutorial.dll"

Microsoft (R) .NET Framework Strong Name Utility  Version 4.0.30319.0
Copyright (c) Microsoft Corporation.  All rights reserved.

Verification entry added for assembly 'ElementTutorial,2D58152B8E842BE2'

C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.8 Tools>
```

After successful running the sn-tool there should be a new key in the regitry:
```
HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Microsoft\StrongName\Verification\ElementTutorial,2D58152B8E842BE2
```
or on 32bit systems
```
HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\StrongName\Verification\ElementTutorial,2D58152B8E842BE2
```

Helpful if you have to deploy the dll on many nodes: You can export this registry key to a file and import it on other nodes. This saves you from installing the .Net Framework Tools everywhere.
