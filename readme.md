# Praktyki


## Pojęcia:
### Część pierwsza

#### VSS (Volume Shadow Copy)
Mechanizm w systemach Windows, który pozwala na tworzenie kopii zapasowych i przywracanie danych, nawet jeśli pliki są w użyciu.

#### Deduplikacja
Proces eliminowania zduplikowanych danych w celu zaoszczędzenia przestrzeni na dysku.

#### Kompresja
Zmienianie rozmiaru pliku przez usunięcie nadmiarowych danych w celu zaoszczędzenia miejsca lub przyspieszenia transferu.

#### Rodzaje kompresji
- **Bezstratna** – dane mogą zostać przywrócone do pierwotnej postaci.
- **Stratna** – część danych zostaje utracona, co powoduje zmniejszenie jakości (np. w kompresji audio czy wideo).

#### Stopnie kompresji
Określają, jak mocno plik ma być skompresowany, często w zależności od kompromisu między jakością a rozmiarem.

#### Szyfrowanie (typy)
Metody zabezpieczania danych przed dostępem osób niepowołanych. Szyfrowanie może być symetryczne lub asymetryczne.

#### Szyfrowanie symetryczne
Używa tego samego klucza do szyfrowania i deszyfrowania danych.

#### Algorytm AES
Jedno z najczęściej używanych szyfrowań symetrycznych. Jest szybki i bezpieczny.

#### Tryby - ECB, CBC
- **ECB (Electronic Codebook)** – każda blokowa jednostka danych jest szyfrowana osobno, co może prowadzić do problemów z bezpieczeństwem.
- **CBC (Cipher Block Chaining)** – blok danych jest zależny od poprzedniego, co zwiększa bezpieczeństwo.

#### Klucz domyślny i klucz użytkownika w kontekście naszej aplikacji
- **Klucz domyślny** – standardowy klucz wykorzystywany przez aplikację.
- **Klucz użytkownika** – klucz wprowadzany przez użytkownika, zwiększający poziom bezpieczeństwa.

#### Retencja plików
Polityka przechowywania plików przez określony czas, z uwzględnieniem ich usuwania po upływie okresu przechowywania.

#### Kopie zapasowe - pełne, przyrostowe i różnicowe
- **Pełne** – kopia całości danych.
- **Przyrostowe** – kopia tylko nowych lub zmodyfikowanych danych.
- **Różnicowe** – kopia danych zmienionych od ostatniej pełnej kopii.

#### Wykonywanie kopii zapasowych na prawach użytkownika
Kopie zapasowe wykonywane przy użyciu uprawnień określonego użytkownika systemu.

#### Zmienne środowiskowe
Parametry systemowe, które wpływają na sposób działania aplikacji i procesów systemowych.

#### Harmonogram
- **Systemowy** – planowanie zadań na poziomie systemu operacyjnego.
- **Obecny w aplikacji** – planowanie działań wewnątrz aplikacji.

#### Pliki ukryte
Pliki systemowe, które nie są widoczne w normalnym widoku katalogu.

#### Filtry plików
Mechanizmy, które umożliwiają wybór plików na podstawie ich typu lub innych cech.

#### Usługa systemowa (poziomy/rodzaje uprawnień)
Usługi działające w tle systemu operacyjnego, z różnymi poziomami uprawnień dostępu.

#### Active Directory
System katalogowy stosowany w Windows do zarządzania użytkownikami, komputerami i innymi zasobami w sieci.

#### Wirtualizacja
Proces tworzenia wirtualnych wersji zasobów komputerowych, takich jak maszyny wirtualne. Typy wirtualizatorów to np. VMware, Hyper-V.

#### Redundancja
Zabezpieczenie danych lub usług poprzez ich duplikowanie, aby zapewnić dostępność w razie awarii.

#### Chmura
Zdalne przechowywanie danych i usług dostępnych przez Internet.

### Część druga

#### NAS (Network Attached Storage)
Urządzenie do przechowywania danych w sieci, pozwalające na dostęp do nich z wielu urządzeń.

#### QNAP, Synology, Asustor, Netgear
Producenti urządzeń NAS, oferujący różnorodne rozwiązania do przechowywania danych.

#### Mono
Implementacja środowiska .NET, pozwalająca na uruchamianie aplikacji .NET na systemach innych niż Windows.

#### Systemy wersjonowania
Narzedzia do zarządzania historią zmian w kodzie źródłowym, np. **Git**.

#### Docker, Kubernetes
- **Docker** – narzędzie do tworzenia i uruchamiania kontenerów.
- **Kubernetes** – system orkiestracji kontenerów.

#### Strony wspierające wersjonowanie w chmurze
Platformy takie jak **GitHub** umożliwiają przechowywanie i wersjonowanie kodu w chmurze.

#### Docker, Kubernetes

- **Docker** – narzędzie do tworzenia, uruchamiania i zarządzania kontenerami. Umożliwia spakowanie aplikacji i jej zależności w jedną jednostkę (kontener), co zapewnia łatwiejsze wdrażanie i przenoszenie aplikacji między różnymi środowiskami.

- **Kubernetes** – system orkiestracji kontenerów, który automatycznie zarządza, skalować i wdraża aplikacje w kontenerach. Kubernetes pozwala na łatwe zarządzanie rozproszonymi aplikacjami w dużych środowiskach produkcyjnych, oferując takie funkcje jak load balancing, skalowanie, automatyczne aktualizacje i zarządzanie stanem aplikacji.

#### Strony wspierające wersjonowanie w chmurze (GitHub i inne)

- **GitHub** – popularna platforma do hostowania projektów opartych na systemie kontroli wersji Git. GitHub umożliwia współpracę nad kodem, śledzenie zmian, zgłaszanie problemów, tworzenie pull requestów oraz integrację z narzędziami CI/CD.

- **GitLab** – podobna do GitHub platforma, oferująca zarządzanie kodem źródłowym z dodatkowymi funkcjami, takimi jak zintegrowane narzędzia CI/CD, zarządzanie projektami, monitorowanie i bezpieczeństwo.

- **Bitbucket** – platforma stworzona przez Atlassian, która oferuje hosting repozytoriów Git i Mercurial, z integracją z innymi narzędziami tej firmy, jak Jira czy Trello. Umożliwia również tworzenie prywatnych repozytoriów.

- **SourceForge** – jedna z pierwszych platform do hostowania projektów open-source, która zapewnia repozytoria Git, SVN i Mercurial. SourceForge oferuje również narzędzia do zarządzania projektami i śledzenia problemów.

#### Klastry
Zestaw współpracujących ze sobą komputerów (serwerów), które wspólnie realizują zadania.

#### Centra Danych
Fizyczne obiekty przechowujące dużą liczbę serwerów i urządzeń służących do przechowywania danych.

#### RAID (rodzaje)
- **RAID 0** – striping (rozdzielanie danych na różne dyski).
- **RAID 1** – mirroring (lustrzane kopie).
- **RAID 5** – striping z parzystością (wysoka dostępność i szybkość).

#### Systemy plików
- **FAT** – starszy system plików, używany głównie w urządzeniach przenośnych.
- **NTFS** – system plików Windows, wspiera dużą liczbę funkcji.
- **ext** – system plików stosowany w Linuxie.

#### Rejestr systemowy Windows
Baza danych przechowująca ustawienia konfiguracyjne systemu Windows.

#### Foldery systemowe
- **Windows** – foldery takie jak `Program Files` czy `Windows`.
- **Linux** – foldery takie jak `/etc` czy `/bin`.

#### Amazon Web Service (S3, EC2), Azure
Usługi chmurowe oferujące przechowywanie danych (S3) i moc obliczeniową (EC2 w AWS, maszyny wirtualne w Azure).

#### OpenStack
Otwarta platforma do zarządzania chmurą prywatną.

### Część trzecia

#### Linki symboliczne
Odwołania do innych plików lub katalogów w systemie plików, pozwalające na ich łatwiejsze odnalezienie.

#### Specjalne pliki systemowe
- **pagefile.sys** – plik używany do zarządzania pamięcią wirtualną w systemie Windows.
- **hiberfil.sys** – plik używany do przechowywania stanu systemu podczas hibernacji.
- **swapfile.sys** – plik wykorzystywany do wymiany pamięci w systemach Windows.

#### Rejestr zdarzeń Windows
System logowania, który zapisuje informacje o błędach, ostrzeżeniach i innych zdarzeniach w systemie.

#### Grupy robocze Windows
Metoda organizowania komputerów w sieci, pozwalająca na współdzielenie zasobów.

#### IPv4, IPv6
- **IPv4** – starsza wersja protokołu internetowego, używająca 32-bitowych adresów.
- **IPv6** – nowsza wersja, używająca 128-bitowych adresów.

#### MSI, EXE
- **MSI** – format instalatora dla systemu Windows.
- **EXE** – format pliku wykonywalnego w systemie Windows.

#### Skrypty .BAT, skrypty .sh
- **.BAT** – skrypty wykonywalne w systemie Windows.
- **.sh** – skrypty powłokowe w systemach Unix/Linux.

#### Ograniczenia systemów w kontekście ograniczeń dotyczących ścieżek do plików
Systemy operacyjne mają limity długości ścieżek do plików (np. 260 znaków w Windows).

#### Typy plików a rozszerzenia
Rozszerzenia plików (.txt, .exe) wskazują na ich format i sposób obsługi.

#### DLL a EXE
- **DLL** – biblioteka dynamiczna, zawiera funkcje wykorzystywane przez różne aplikacje.
- **EXE** – plik wykonywalny, uruchamiany przez system operacyjny.

#### Procesy a wątki
- **Proces** – niezależna jednostka wykonawcza.
- **Wątek** – najmniejsza jednostka wykonawcza, część procesu.

#### Wielowątkowość
Zdolność procesora do wykonywania wielu zadań jednocześnie, zwiększająca efektywność aplikacji.

#### Grafika wektorowa a rastrowa
- **Wektorowa** – obrazy oparte na matematycznych równaniach.
- **Rastrowa** – obrazy oparte na pikselach.

#### Bitlocker
Funkcja szyfrowania dysku w systemach Windows, zabezpieczająca dane przed nieautoryzowanym dostępem.

#### HDD a SSD
- **HDD** – tradycyjny dysk twardy, bazujący na talerzach magnetycznych.
- **SSD** – dysk półprzewodnikowy, szybszy i bardziej niezawodny.

#### UEFI a BIOS
- **UEFI** – nowoczesna wersja BIOS, wspiera większe dyski i szybsze uruchamianie systemu.
- **BIOS** – starszy system uruchamiania komputera.
