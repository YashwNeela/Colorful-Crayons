cd Assets
git clone --recurse-submodules https://github.com/YashwNeela/Colorful-Crayons
cd Colorful-Crayons
git config core.sparseCheckout true
echo Assets/_SortingGame/_Scripts/ >> .git/info/sparse-checkout
git read-tree -mu HEAD

