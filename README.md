# Quick Print Studio 1.0 — CorelDRAW 2025

**Produção gráfica assistida, dentro do CorelDRAW.**

Quick Print Studio é um addon open-source para CorelDRAW 2025 64-bit (API v26) criado para reduzir tarefas repetitivas de pré-impressão e tornar operações técnicas compreensíveis para quem está começando, sem esconder controles úteis de operadores experientes.

> **Status de validação:** o motor independente, cálculos e arquivos de interface podem ser testados em CI. Recursos que manipulam o CorelDRAW exigem build e validação em uma máquina Windows com CorelDRAW 2025/VGCore instalado. O projeto não distribui bibliotecas proprietárias da Corel.

## Quick start

1. Instale o CorelDRAW 2025 64-bit.
2. Em uma máquina de desenvolvimento, execute `build\Build.ps1`.
3. O script executa testes, valida XAML/XSLT, detecta CorelDRAW/VGCore e compila o addon.
4. Com NSIS instalado, o mesmo script gera `QuickPrintStudio-1.0.0-Setup.exe`.
5. O instalador valida a instalação do CorelDRAW antes de copiar qualquer arquivo.

## O que existe na 1.0

| Área | Recursos |
|---|---|
| Início | busca em linguagem simples, resumo e fluxo guiado |
| Preflight | arquitetura para RGB, resolução, fontes e sangria; explicações em linguagem simples |
| Produção | presets para gráfica geral, cartão, adesivo e grande formato |
| Imposição | cálculo de colunas, linhas, quantidade por folha e base para Step & Repeat |
| Medidas | largura, altura, área em m² e perímetro |
| QR | tela preparada para PIX, WhatsApp, URL, texto, OS e etiquetas |
| Exportação | nomenclatura segura de arquivos e arquitetura para PDF/PNG/JPG |
| Ajuda | glossário integrado de termos de produção |
| Qualidade | testes unitários, validação XML/XAML, CI e build fail-safe |
| Instalação | detecção do CorelDRAW 2025, desinstalador e cancelamento seguro |

## UX para iniciante e especialista

O iniciante pode pesquisar por intenção — por exemplo, “colocar vários cartões na folha” — e receber a ferramenta correspondente. Termos técnicos são acompanhados de explicações. O profissional pode ir diretamente às abas de Preflight, Produção, Imposição, Medidas e Exportação.

A interface usa rolagem, dimensionamento flexível, navegação por teclado, tooltips e controles sem coordenadas absolutas para funcionar melhor em diferentes tamanhos de Docker e escalas do Windows.

## Arquitetura

```text
CorelDRAW 2025 / VGCore v26
          |
QuickPrintStudio.Addon
WPF • .NET Framework 4.8 • x64
          |
QuickPrintStudio.Core
Preflight • Presets • Imposição • Medidas
Busca • Nomes de produção • Step & Repeat
          |
Testes + CI + Build.ps1 + NSIS
```

O host permanece em .NET Framework por compatibilidade com WPFHost do CorelDRAW. As regras independentes ficam isoladas no Core para permitir testes sem abrir o CorelDRAW.

## Segurança de documento

Operações futuras que alteram objetos devem usar grupos de comando com encerramento garantido em `finally`, para que uma ação possa ser desfeita em um único Undo e para proteger a pilha de desfazer do CorelDRAW. Nenhuma rotina deve alterar uma arte silenciosamente.

## Estrutura do repositório

```text
src/
  QuickPrintStudio.Core/
  QuickPrintStudio.Addon/
tests/
docs/
installer/
build/
.github/workflows/
VERSION
```

## Testes

```powershell
dotnet test .\tests\QuickPrintStudio.Core.Tests\QuickPrintStudio.Core.Tests.csproj -c Release
```

A suíte cobre imposição, entradas inválidas, preflight, busca em linguagem natural, nomes de arquivo, medidas, Step & Repeat e presets. A CI valida também o XAML, o XSLT, o manifesto e a versão.

## Build

```powershell
powershell -ExecutionPolicy Bypass -File .\build\Build.ps1
```

O script:
- executa os testes antes do build;
- valida arquivos de interface;
- detecta CorelDRAW 2025;
- localiza `Corel.Interop.VGCore.dll` na instalação local;
- compila x64;
- prepara `dist\addon`;
- gera o instalador NSIS quando `makensis.exe` está disponível.

## Instalador

O NSIS gera **QuickPrintStudio-1.0.0-Setup.exe**. O instalador não tenta “adivinhar” silenciosamente: se não encontrar `CorelDRW.exe` de uma instalação compatível, cancela sem instalar o addon.

O padrão oficial de addons do CorelDRAW usa uma pasta sob `Programs64\Addons`, um arquivo vazio `CorelDrw.addon` para indicar carregamento e arquivos XSLT para integração de UI.

## Documentação

- `docs/USER-GUIDE.md` — guia para iniciantes e profissionais.
- `docs/ARCHITECTURE.md` — decisões de arquitetura, segurança e limites de validação.

## Compatibilidade

| Componente | Alvo |
|---|---|
| CorelDRAW | 2025 |
| API | v26 |
| Arquitetura | x64 |
| Host | WPF |
| Framework do host | .NET Framework 4.8 |
| Core independente | .NET Standard 2.0 |
| Instalador | NSIS |
| Sistema | Windows compatível com CorelDRAW 2025 |

## Pesquisa e referências

A implementação foi projetada a partir da documentação oficial do CorelDRAW SDK e do estudo de addons/macros públicos maduros. Ideias foram reimplementadas de forma própria; o projeto não depende de copiar código de terceiros para formar o núcleo.

Referências principais: CorelDRAW SDK v26, documentação de Custom Add-ons, Custom Dockers e FrameWork.AddDocker.

## Licença e contribuições

Antes de distribuir publicamente como produto final, defina uma licença explícita no repositório e valide a integração em uma instalação real do CorelDRAW 2025. Pull requests devem manter a separação entre Core e VGCore, incluir testes para regras novas e não adicionar binários proprietários da Corel.

---

**Versão:** 1.0.0  
**Projeto:** Quick Print Studio  
**Alvo:** CorelDRAW 2025 64-bit
