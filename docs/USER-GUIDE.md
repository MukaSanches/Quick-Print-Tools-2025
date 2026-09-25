# Guia do Usuário — Quick Print Studio 1.0

## Para quem está começando
Abra o painel **Quick Print Studio** no CorelDRAW. A tela inicial foi organizada por intenção: analisar, preparar, organizar, medir, gerar QR e exportar. Cada ferramenta mostra uma explicação curta antes da execução.

### Fluxo recomendado
1. Abra sua arte.
2. Use **Analisar documento**.
3. Corrija itens marcados como **Erro** e revise os itens **Atenção**.
4. Entre em **Produção** para configurar sangria, marcas e organização.
5. Use **Medidas** para conferir tamanho, área e perímetro.
6. Use **Exportar** para gerar o arquivo de produção.

## Para profissionais
O painel oferece acesso direto a preflight, presets, imposição, step & repeat, medidas, exportação e lote. A arquitetura separa regras de negócio da integração VGCore para manter testes rápidos e reduzir risco de regressão no CorelDRAW.

## Segurança
Ações destrutivas devem ser agrupadas em um único Undo. A integração CorelDRAW deve sempre finalizar grupos de comando mesmo em caso de erro, conforme recomendação da própria API.

## Telas
- **Início**: resumo do documento, busca e atalhos.
- **Preflight**: erros, avisos e explicações.
- **Produção**: sangria, marcas, margens e presets.
- **Imposição**: cálculo de aproveitamento e step & repeat.
- **Medidas**: largura, altura, área e perímetro.
- **QR Code**: espaço preparado para QR vetorial de PIX, WhatsApp, URL e texto.
- **Exportar**: presets e nomenclatura de produção.
- **Lote**: fila de arquivos e status.
- **Ajuda**: glossário para iniciantes e fluxo rápido para especialistas.

## Glossário
**Sangria**: sobra da arte além do corte.  
**CMYK**: modelo de cor comum em impressão.  
**RGB**: modelo de cor usado principalmente em telas.  
**DPI**: medida usada para avaliar resolução de imagens.  
**Imposição**: organização de várias artes em uma folha.  
**PowerClip**: conteúdo inserido dentro de outro objeto no CorelDRAW.  
**Preflight**: verificação técnica antes da produção.
