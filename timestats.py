from __future__ import with_statement

def main():
    total = 0.0
    with open("time.txt") as f:
        for line in f:
            print line.strip()
            try:
                time = float(line.split(":")[1].strip())
                print "%.2f hours\n" % time
                total += time
            except:
                print "Cannot read value"
                break
    
    print "Total: %.2f" % total
            
main()