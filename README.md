# Quick Print Studio for CorelDRAW 2025

Addon de produção gráfica para CorelDRAW 2025 (API v26), projetado para ser compreensível para iniciantes e eficiente para profissionais.

## Visão
O Quick Print Studio reúne preflight, produção, geometria, imposição, QR, busca em linguagem simples, presets e exportação em um Docker integrado ao CorelDRAW. A UI usa layout responsivo, rolagem vertical, foco por teclado, tooltips explicativos e base para temas.

## Estado atual
A fundação v0.1 foi implementada com:
- motor independente de preflight;
- cálculo de imposição;
- busca universal em linguagem simples;
- nomenclatura segura de arquivos de produção;
- Docker WPF responsivo;
- manifesto CorelDRAW (CorelDrw.addon + AppUI.xslt);
- instalador NSIS com detecção do CorelDRAW 2025;
- build script que detecta Corel e VGCore;
- testes unitários e CI.

As ações que alteram documentos CorelDRAW permanecem desabilitadas até a integração VGCore ser compilada e validada em uma máquina Windows com CorelDRAW 2025 instalado. Isto é intencional: o projeto não finge uma integração que não foi testada.

## Arquitetura
- QuickPrintStudio.Core: regras testáveis sem CorelDRAW.
- QuickPrintStudio.Addon: WPF/.NET Framework 4.8 hospedado no Corel.
- installer: instalador NSIS.
- tests: testes do motor.
- build: build local em máquina com CorelDRAW 2025.

## Build real do addon
Em Windows com CorelDRAW 2025 instalado:

    powershell -ExecutionPolicy Bypass -File .\build\Build.ps1

O script procura o CorelDRAW 2025 e a Corel.Interop.VGCore.dll, compila x64 e prepara dist\addon.

## Instalador
Após gerar dist\addon, compile installer\QuickPrintStudio.nsi com NSIS. O instalador procura o CorelDRAW 2025, valida CorelDRW.exe e instala em Programs64\Addons\QuickPrintStudio. Se não encontrar uma instalação válida, cancela com segurança.

## Testes
    dotnet test .\tests\QuickPrintStudio.Core.Tests\QuickPrintStudio.Core.Tests.csproj -c Release

A CI também valida XML/XAML dos arquivos principais.

## Referências de engenharia
A arquitetura segue o modelo de add-ons do SDK oficial do CorelDRAW: CorelDrw.addon, XSLT de UI e Docker .NET. Ideias funcionais foram estudadas em CdrTools, CdrPreflight, DockerTemplateX7, DropShadowDocker, Bonus630DevToolsBar e QrCodeDocker. Código de terceiros não foi simplesmente copiado; a base é uma implementação própria para manter arquitetura, manutenção e licenciamento controláveis.

## Roadmap imediato
1. Adapter VGCore v26 e eventos de documento/seleção.
2. Preflight real de objetos RGB, bitmaps/DPI, fontes, outlines e bleed.
3. Localizar objeto do problema dentro do Corel.
4. Command groups/Undo seguro.
5. Marcas de corte e sangria.
6. Imposição/step-and-repeat.
7. Área/perímetro/offset.
8. QR vetorial.
9. Presets de produção.
10. Exportação PDF/PNG/JPG.
11. Batch com cancelamento e progresso.
12. Integração opcional com Quick Print OS.

## Compatibilidade
Alvo principal: CorelDRAW 2025 64-bit / API v26. O host WPF usa .NET Framework 4.8 por compatibilidade com o processo do CorelDRAW.
