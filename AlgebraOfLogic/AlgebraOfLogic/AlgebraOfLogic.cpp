

#include <iostream>
using namespace std;



int recFunc(int n) {
    if (n == 0) {
        int res;
        cin >> res;
        return res;
    }
    for (size_t i = 0; i < 2; i++)
    {
        cout << i;
        recFunc(n - 1);
    }

}

int main()
{
    int n;
    cout << n;
    recFunc(n);
}


