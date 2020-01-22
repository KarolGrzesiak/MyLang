# MyLang 
## Code is not pretty or clean by any means, that was a quick project for university class. Description is in Polish.
 
------------------- Schemat -------------------


![alt text](http://www.plantuml.com/plantuml/png/JOy_2uD03CNtV8hWhX-WGx5qwDAXe9iuX9gfDWUv4_e7ltiZ8LHkoNllbpokpOMqBCyP94sCrOM5bKhI2h-0E9pDrH5jzFn6cbchBRFq93I_FmO0kkvw7ntkxYJofw_y2dJequDbX7LJiDg3g6gQu-um7d3933kZhhqdlDZnyx4GokZeQbac--WF)

------------------- OPIS -------------------

Lexer przy tworzeniu otrzymuję ścieżkę do pliku który trzeba skompilować.
Poprzez wywołanie metody GetTokens() lexer klasyfikuje odpowiednie tokeny i zwraca
ich listę. Wykorzystuję przy tym klasę Token opisującą dany token - jakiego jest typu,
gdzie został spotkany w tekście oraz jego wartość czyli co oznacza. Dodatkowo w tym pliku
występuje też enum TokenKind czyli możliwe typy tokenów.

Analyzer przy tworzeniu otrzymuję listę tokenów.
Poprzez wywołanie metody Parse() zaczyna budować AST czyli drzewo składniowe. Gdzie root
to Compound czyli najbardziej nadrzędny syntax, najbardziej złożony, dalej dzieli się on
na odpowiednio mniejsze etykiety.
Na podstawie listy tokenów drzewo jest rozbudowywane poprzez odpowiednią klasyfikację
tzn. tokeny są zamieniane na odpowiednie etykiety drzewa zdefiniowane w pliku AST.
Te etykiety są łączone ze sobą w wyniku czego powstaje drzewo. Etykiety oczywiście posiadają
również wartości danych tokenów.

Coder przy tworzeniu otrzymuje AST, a dokładnie referencję do jego roota.
Poprzez wywołanie metody GenerateCode() Coder przystępuje do przechodzenia przez drzewo
(wykorzystuję metodę Visit() obiektu NodeVisitor(), która na podstawie odczytanej
etykiety drzewa wywołuje odpowiednią metodę obiektu Coder) generując program
w języku python - który można zapisać do pliku. Każda etykieta jest odczytywana wraz
z wartością jaką ze sobą niesie i na tej podstawie tłumaczona jest na język
pythonowy.

------------------- Użycie -------------------

Do działania kompilatora potrzebny jest .NET Core SDK

W pliku Program.cs ustawiamy odpowiednią ścieżkę do pliku źródłowego
który ma być skompilowany.
Mając .NET Core SDK, wewnątrz folderu Lang z konsoli uruchamiamy polecenie 'dotnet run'.
W wyniku tej operacji w folderze Lang powstaje plik result.py będący skompilowanym
plikem źródłowym do języka python.
