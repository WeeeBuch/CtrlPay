# Core
Tento projekt obsahuje hlavní bussiness logiku

## AuthLogic
Zde je logika přihlášení, změny hesla, registrace a 2FA TOTP autentizace
Využívá token service z api pro generování a využití JWT tokenů

## PaymentProcessing
Zde jsou metody pro párování transakcí a plateb, dokončování transakcí na hlavní adresy, aby se dále nepokoušely párovat, označování plateb jako expirované a placení plateb z kreditů

## XMRComs
Zde jsou metody pro práci s monero peněženkou, resp. se tu volají metody z projektu XMR

# XMR
Zde probíhá samotná komunikace s monero peněženkou
Jsou zde metody pro synchronizaci transakcí, adres a accountů z monero peněženky a metody pro vytváření nových adres a accountů