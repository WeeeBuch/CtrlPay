# Api
Tento soubor popisuje funkci api

## Struktura
Api se skládá především z controllerů, dále obsahuje služby na pozadí a službu pro vydávání JWT tokenů a službu pro odesílání emailů

## Controllery
Slouží k manipulaci s entitami v databázi
Téměř všechny controllery se jmenují podle entit se kterými pracují.
Hlubší bussiness logika se děje přes metody z Core

### Account Controller
Zde se pracuje s accounty v XMR peněžence
Jsou zde metody k získání nové adresy a k získání hlavní adresy accountu

### Auth Controller
Zde je hlavní logika autentizace
Jsou zde metody pro login, registraci a příprava na 2FA TOTP autentizaci

### Customer, Payment, Transaction Controller
Slouží k práci s entitou dle názvu, jsou zde metody pro CRUD
Customer má navíc metodu na promotování zákazníka z běžného na loyal
Payment má navíc metody pro převedení přeplatku na kredit, zaplacení platby z kreditu a získání různě profiltrovaných plateb.


## Background services
Slouží k opakovanému spouštění procesů na pozadí

### Payment processing
Zde se spouští metody pro párování plateb s transakcemi, označování plateb jako expirované a dokončovat transakce na primární adresu, aby se už nepokoušely párovat

### XMR Coms
Zde se spouští metody na synchronizaci aktuálního stavu peněženky a databáze
Synchronizují se zde accounty a transakce

## Ostatní services

### Email service
Slouží pro renderování konkrétních mailů z Razor pages šablon a odesílání těchto emailů

### Token service
Zde je logika generování JWT tokenů, správa refresh tokenů a jwt validaci