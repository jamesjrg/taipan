#!/usr/bin/env python

def main():
    total = 0.0
    with open("../time.txt") as f:
        for line in f:
            print line
            if line.startswith('#'):
                continue
            try:
                time = float(line.split(":")[1].strip())
                print "%.2f hours\n" % time
                total += time
            except:
                print "Cannot read value"
                break
    
    print "Total: %.2f" % total
            
main()
