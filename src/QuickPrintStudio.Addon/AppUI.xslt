<?xml version="1.0"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:frmwrk="Corel Framework Data">
  <xsl:output method="xml" encoding="UTF-8" indent="yes"/>
  <frmwrk:uiconfig><frmwrk:applicationInfo userConfiguration="true"/></frmwrk:uiconfig>
  <xsl:template match="node()|@*"><xsl:copy><xsl:apply-templates select="node()|@*"/></xsl:copy></xsl:template>
  <xsl:template match="uiConfig/items">
    <xsl:copy><xsl:apply-templates select="node()|@*"/>
      <itemData guid="b2a39027-681c-4fa3-a362-416ad15f58c8" type="wpfhost" hostedType="Addons\\QuickPrintStudio\\QuickPrintStudio.dll,QuickPrintStudio.Addon.Views.DockerView" caption="Quick Print Studio"/>
    </xsl:copy>
  </xsl:template>
</xsl:stylesheet>