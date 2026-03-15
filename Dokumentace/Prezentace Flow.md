# Flow našeho prezentování

## Předpříprava

| Kdo | Co ukazuje | Na čem ukazuje | Zájmeno |
| --- | --- | --- | --- |
| Kryštof | Avalonia Desktop | Pc s projektorem | Local |
| Karel | Api a DB | Kolosh Proxomoxo | Cloud |
| Adel | Avalonia Android | Pc nebo na mobilu | Mobil |

### Připravit si:
- [ ] VMware stanici pro local
	- [ ] Desktop aplikaci nainstalovanou
	- [ ] Mít otevřeného swaggera
- [ ] Kulosh stanici
	- [ ] Desktop aplikace
	- [ ] Monero node
	- [ ] Monero GUI Wallet
	- [ ] HeidySQL
- [ ] Mobil na pc nebo apka na telefonu

---

## Účty

| Name | Password     | Role |
| --- | --- | --- |
|  Adam   |  123456Ab    | Admin |
| Sedita | 123456Ab | Accountant |
| NovakJan | 123456Ab | Customer |

---

## Prezentace Flow

### 1. První spuštění 
Na `Local` se pustí poprvé aplikace (nebude mít settings aby se spustilo onbording)
Potom se přihlásí `admin`

#### Ukazat
- Spustit aplikaci
- Vybrat oranžovou barvu
- Nechat jazyk na CZ
- Připojit k API `https://www.action-games.cz`
- Testnout konekci
- Přihlásit jako `adam`

### 1.1 Upravit nastavení
Na `locale` si upravit nastavení

#### Ukazat
- Settings přepnout na `Lime`
- Translation na `Pirate`
- Obovování na `10 sec`

### 2. Crud PC1 (Admin)
`Admin` udělá uživatele sekretářce a ukáže že může dělat CRUD

#### Ukazat
- Vytvořit uživatele `sedita` s heslem
- Vytvořit uživatele `horalka` s heslem

### 3. Sekretářka
Na `Mobilu` a `Localu` se sekretářka přihlásí

#### Ukazat
- Na `mobilu` se přihlásí `sedita`

### 4. Novej Zákazník CRUD Mobil1
Na `Mobilu` sekretářka udělá nového `Customera`

#### Ukazat
- `Sedita` vytvoří zákazníka `Jan Novák`
- Promotne ho
- `Cloud` se přihlásí přes odkaz v mailu
- Zvorá heslo
- `Admin` změní heslo Novákovi

### 5. Zákazník přihlášení
Na `Cloudu` se zákazník přihlásí pomocí poskytnutého kódu od sekretářky

### 6. CRUD PC2 a Mobil2 (Transakce)
Na `PC` a `Mobilu` se udělají transakce na zaplacení

### 7. Přeplacení
Na `Cloudu` zákazníkovy se zobrazí že má zaplatit
Vygeneruje si adresu a zaplatí víc (alespoň tolik co obě dohromady)

### 8. Na kredity
Na `Mobilu` se sekretářce zobrazí že je přeplaceno
Klikne a nechá převédst na kredity ten přeplatek

### 9. DB
Na `Cloudu` se ukáže že se to převedlo

### 10. Kredity
Na `Cloudu` zaplatí z kreditů

### 11. Sekretářka vidí 
Bude vydět že se zaplatilo