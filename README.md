# changeVer
All change FileVersion/ProductVersion on MFC resource files

## Enveroment
* Virsual Studio 2015
* C#

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
