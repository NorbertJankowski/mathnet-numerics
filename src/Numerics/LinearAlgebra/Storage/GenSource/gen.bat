copy DataTableStorageBase.cs ..\DataTableStorage.cs
type DataTableStorageFloat.cs >> ..\DataTableStorage.cs

gawk "{ gsub(/Float/, \"Double\"); print }" < DataTableStorageFloat.cs > tmp.cs
gawk "{ gsub(/float/, \"double\"); print }" < tmp.cs >> ..\DataTableStorage.cs

gawk "{ gsub(/Float/, \"Byte\"); print }" < DataTableStorageFloat.cs > tmp.cs
gawk "{ gsub(/float/, \"byte\"); print }" < tmp.cs >> ..\DataTableStorage.cs

gawk "{ gsub(/Float/, \"Bool\"); print }" < DataTableStorageFloat.cs > tmp.cs
gawk "{ gsub(/float/, \"bool\"); print }" < tmp.cs >> ..\DataTableStorage.cs

gawk "{ gsub(/Float/, \"Complex\"); print }" < DataTableStorageFloat.cs > tmp.cs
gawk "{ gsub(/float/, \"Complex\"); print }" < tmp.cs >> ..\DataTableStorage.cs

gawk "{ gsub(/Float/, \"Complex32\"); print }" < DataTableStorageFloat.cs > tmp.cs
gawk "{ gsub(/float/, \"Complex32\"); print }" < tmp.cs >> ..\DataTableStorage.cs




echo qniec