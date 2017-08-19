copy DataTableStorageBase.cpp ..\DataTableStorage.cpp
type DataTableStorageFloat.cpp >> ..\DataTableStorage.cpp

gawk "{ gsub(/Float/, \"Double\"); print }" < DataTableStorageFloat.cpp > tmp.cpp
gawk "{ gsub(/float/, \"double\"); print }" < tmp.cpp >> ..\DataTableStorage.cpp

gawk "{ gsub(/Float/, \"Byte\"); print }" < DataTableStorageFloat.cpp > tmp.cpp
gawk "{ gsub(/float/, \"byte\"); print }" < tmp.cpp >> ..\DataTableStorage.cpp

gawk "{ gsub(/Float/, \"Bool\"); print }" < DataTableStorageFloat.cpp > tmp.cpp
gawk "{ gsub(/float/, \"bool\"); print }" < tmp.cpp >> ..\DataTableStorage.cpp

gawk "{ gsub(/Float/, \"Complex\"); print }" < DataTableStorageFloat.cpp > tmp.cpp
gawk "{ gsub(/float/, \"Complex\"); print }" < tmp.cpp >> ..\DataTableStorage.cpp

gawk "{ gsub(/Float/, \"Complex32\"); print }" < DataTableStorageFloat.cpp > tmp.cpp
gawk "{ gsub(/float/, \"Complex32\"); print }" < tmp.cpp >> ..\DataTableStorage.cpp



echo qniec