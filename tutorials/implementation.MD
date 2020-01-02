## Creating a Graphic Primitive
- Create a new C# class library (.net Framework) project. Chose the .net framework version depending on your 800xA version. You can also use higher Versions, but in this case they have to be installed on the target system.
    - 800xA V5.1 uses .net 3.5
    - 800xA 6.0/6.1 uses .net 4.5
- Get the the public strong name signature key. You can get it from the example project, or extract it from some other graphic primitive assembly. For extraction we are using the dotnet strong name tool. More informations here: https://msdn.microsoft.com/en-us/library/k5b5tt23%28VS.80%29.aspx
This is the complete command:
'sn -e "c:\Program Files (x86)\ABB Industrial IT\Operate IT\Process Portal A\bin\GraphicPrimitives\ABB.xA.ProcessGraphics.AspectViewPrimitives.dll" "PG2Key.snk"
- Add the signature key to your project.
    - go to the project properties... signing
    - check "Sign the assembly", browse for the PG2Key.snk, check "Delay Sign only" too.
- Add a reference to the **GraphicsKernel.dll**, you can usually find this file in **c:\Program Files (x86)\ABB Industrial IT\Operate IT\Process Portal A\bin\**.
- Implement the Class
