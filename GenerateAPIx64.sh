#!/bin/sh

cd $(wslpath $1)

for file in `ls *.txt`; do
	sed -E 's/--stack=[0-9]+ //g' $file > $(basename $file .txt)64.txt
done

cd -
