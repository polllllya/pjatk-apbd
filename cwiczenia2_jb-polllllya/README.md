# Ćwiczenia 2

Zadanie polega na napisniu aplikacji odczytującej plik `CSV` z listą studentów po czym przefiltrowaniu jej i usunięciu z niej duplikatów oraz błędnych rekordów. Następnie należy zapisać plik w formacie `JSON`.

## Działanie aplikacji w skrócie
* Program otrzymuje dwa argumenty - lokalizację pliku wejściowego oraz lokalizację pliku wyjściowego.
* Następnie odczytywany jest plik i filtrowane są duplikaty studentów.
* Ostatecznie w wybranej lokalizacji tworzony (lub nadpisywany) jest nowy plik bez duplikatów.

## Wymagania
* System powinien odczytywać studentów z pliku wejściowego
* System powinien zapisywać tylko unikatowe rekordy do finalnego pliku.
* System powinien zapisywać wszystkie wyjątki oraz błędy, które powstały do pliku `log.txt`.
* W przypadku gdy system napotka studenta, który już istnieje, ale na innych studiach, powinien dopisać jedynie nowe studia do już istniejęcego rekordu.
* W pliku wyjściowym s-ka studenta powinna zaczynać się od litery `s`

## Uwagi:
* Pola w pliku `.csv` to odpowiednio: `Imię`, `Nazwisko`, `Kierunek studiów`, `Tryb studiów`, `S-ka`, `Data urodzenia`, `Email`, `Imię matki`, `Imię ojca`.
* Rekord jest nie unikatowy, gdy ma tą samą S-kę, Imię i Nazwisko oraz Kierunek i Tryb studiów co inny student. 
* Błędny rekord, to taki rekord, który nie składa się z 9 pól.
* Format danych w pliku wyjściowym to 
```yaml
{
  CreatedAt: datetime,
  Author: string,
  Students: [
      {
        Index: string,
        Name: string
        Surname: string,
        Email: string,
        BirthDate: date,
        MothersName: string,
        FathersName: string,
        Studies: [
           {
              Name: string,
              Mode: string
           }...
        ]
      }...
    ]
}
```

# Pomoc
* Do obsługi plików `CSV` można użyć biblioteki `CsvHelper`
* Do obsługi plików `JSON` można użyć klasy `JsonSerializer`.
