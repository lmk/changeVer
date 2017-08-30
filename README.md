# changeVer
All change FileVersion/ProductVersion on MFC resource files

MFC 프로젝트가 여러개 일때, 버전 변경을 일일이 수정하는 것이 번거롭습니다.
이럴때, 버전일 일괄적으로 변경해 주는 툴입니다.

원리는, 설정 파일에 리소스파일 경로를 넣어둔 목록을 만들고
툴에서 파일 목록을 읽어서 아규먼트로 받은 버전으로 일괄 치환 합니다.
Visual Studio 2015 C# ( .NET Framework 4.5.2 ) 기반에서 코딩해 봤습니다.


## Enveroment
* Virsual Studio 2015
* C# (.NET Framework 4.5.2)

## Help
```
Usage>
$ changeVer.exe -f FileVersion -p ProductVersion -c ConfigFilename
ex) $ changeVer.exe -f 2.1.6.33 -p 2.1.0.0 -c resources.txt
ex) $ changeVer.exe -s -c resources.txt

Options>
        -f[F]: After File version
        -p[P]: After Product version
        -c[C]: Config filename
        -s[S]: Show Current version

```

## How to use

1. Make list file
```
$ type resources.txt
D:\Source\server\server.rc
D:\Source\client\client.rc
D:\Source\client2\client2.rc
D:\Source\client3\client3.rc
```

2. Execute tool
```
$ changeVer -f 2.1.6.34 -p 2.1.0.0 -c resources.txt
OK:     D:\Source\server\server.rc
OK:     D:\Source\client\client.rc
OK:     D:\Source\client2\client2.rc
OK:     D:\Source\client3\client3.rc

$ changeVer -s -c resources.txt
D:\Source\server\server.rc:
         FILEVERSION 2,1,6,34
         PRODUCTVERSION 2,1,0,0
                    VALUE "FileVersion", "2, 1, 6, 34"
                    VALUE "ProductVersion", "2, 1, 0, 0"
D:\Source\client\client.rc:
         FILEVERSION 2,1,6,34
         PRODUCTVERSION 2,1,0,0
                    VALUE "FileVersion", "2, 1, 6, 34"
                    VALUE "ProductVersion", "2, 1, 0, 0"
D:\Source\client2\client2.rc:
         FILEVERSION 2,1,6,34
         PRODUCTVERSION 2,1,0,0
                    VALUE "FileVersion", "2, 1, 6, 34"
                    VALUE "ProductVersion", "2, 1, 0, 0"
D:\Source\client3\client3.rc:
         FILEVERSION 2,1,6,34
         PRODUCTVERSION 2,1,0,0
                    VALUE "FileVersion", "2, 1, 6, 34"
                    VALUE "ProductVersion", "2, 1, 0, 0"
```
