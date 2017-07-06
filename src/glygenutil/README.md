# Examples

### Loading SDF file into MongoDB
```
glygenutil.exe -c load-mongo -i ..\..\..\..\data\DSSTox_Pubchem_20160720.sdf -u mongodb://192.168.1.35 -d GlyGen
```

### Loading CSV file into MongoDB
```
glygenutil.exe -c load-mongo -i ..\..\..\..\data\nci_unique.csv -u mongodb://192.168.1.35 -d GlyGen
```

### Loading XML file into MongoDB
```
glygenutil.exe -c load-mongo -i ..\..\..\..\data\medline16n0001.xml -e MedlineCitation -u mongodb://192.168.1.35 -d GlyGen
```
