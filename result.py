x=2
y=3
def test(a,b):
    if (a>b):
        return a

    if (b>a):
        return b

    if (a==b):
        return 0


def test2(c):
    if (c!="test"):
        print("dziala")


def test3():
    print("elo")

while (x<=y):
    print(test(x,y))
    y=y-1

boolVar=True
if (boolVar==True):
    print(boolVar)

test2(1)
test3()
