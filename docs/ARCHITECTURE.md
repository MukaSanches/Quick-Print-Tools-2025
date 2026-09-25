# Arquitetura 1.0

Quick Print Studio usa quatro limites claros:

1. **Core** — cálculos, regras, presets, busca e modelos independentes do CorelDRAW.
2. **Addon Host** — WPF em .NET Framework 4.8, carregado como WPFHost.
3. **Corel Adapter** — integração exclusiva com VGCore v26. Nenhuma regra de negócio deve depender diretamente de COM.
4. **Packaging** — script de build, validação e NSIS.

## Princípios
- x64 e CorelDRAW 2025 como alvo principal.
- nenhum binário proprietário da Corel é versionado no GitHub;
- build local detecta a instalação e referencia VGCore da máquina;
- toda operação mutável deve ser Undo-safe;
- não modificar documento silenciosamente;
- mensagens explicam o impacto para iniciantes;
- atalhos e telas compactas atendem operadores experientes;
- nenhum acesso à internet é necessário para o núcleo do addon.

## Limite de validação
A CI pública testa o Core e valida XAML/XSLT/scripts. O teste de integração real exige Windows com CorelDRAW 2025 e sua biblioteca VGCore instalada.
