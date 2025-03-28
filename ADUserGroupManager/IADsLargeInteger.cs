﻿using System.Runtime.InteropServices;

[ComImport, Guid("9068270b-0939-11d1-8be1-00c04fd8d503")]
[InterfaceType(ComInterfaceType.InterfaceIsDual)]
public interface IADsLargeInteger
{
    int HighPart { get; set; }
    int LowPart { get; set; }
}
