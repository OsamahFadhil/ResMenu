import html2pdf from 'html2pdf.js'
import type { MenuCategory } from '~/stores/restaurant'
import type { TemplateTheme } from '~/stores/templates'

export interface ExportOptions {
  format: 'pdf' | 'html'
  orientation: 'portrait' | 'landscape'
  paperSize: 'a4' | 'letter' | 'legal'
  includeImages: boolean
  includePrices: boolean
  includeDescriptions: boolean
  fileName?: string
}

export const useMenuExport = () => {
  const generateMenuHTML = (
    categories: MenuCategory[],
    theme: TemplateTheme,
    restaurantName: string,
    logoUrl?: string,
    options?: Partial<ExportOptions>
  ): string => {
    const {
      includeImages = true,
      includePrices = true,
      includeDescriptions = true
    } = options || {}

    const fontSizeMap = {
      small: '14px',
      medium: '16px',
      large: '18px'
    }

    const fontSize = fontSizeMap[theme.fontSize || 'medium']

    const html = `
<!DOCTYPE html>
<html>
<head>
  <meta charset="UTF-8">
  <title>${restaurantName} - Menu</title>
  <style>
    * {
      margin: 0;
      padding: 0;
      box-sizing: border-box;
    }

    body {
      font-family: ${theme.fontFamily || 'Arial, sans-serif'};
      font-size: ${fontSize};
      color: ${theme.textColor};
      background-color: ${theme.backgroundColor};
      padding: 40px;
      line-height: 1.6;
    }

    .menu-header {
      text-align: center;
      margin-bottom: 40px;
      padding-bottom: 30px;
      border-bottom: 3px solid ${theme.primaryColor};
    }

    .menu-header img {
      max-width: 150px;
      max-height: 150px;
      margin-bottom: 20px;
    }

    .menu-header h1 {
      font-size: 2.5em;
      color: ${theme.primaryColor};
      margin-bottom: 10px;
      font-weight: bold;
    }

    .category {
      margin-bottom: 50px;
      page-break-inside: avoid;
    }

    .category-header {
      background-color: ${theme.primaryColor};
      color: white;
      padding: 15px 20px;
      margin-bottom: 20px;
      border-radius: 8px;
      font-size: 1.5em;
      font-weight: bold;
    }

    .menu-item {
      display: flex;
      gap: 20px;
      padding: 15px;
      margin-bottom: 15px;
      background-color: ${theme.surfaceColor};
      border-radius: 8px;
      page-break-inside: avoid;
    }

    .item-image {
      width: 100px;
      height: 100px;
      object-fit: cover;
      border-radius: 8px;
      flex-shrink: 0;
    }

    .item-content {
      flex: 1;
    }

    .item-header {
      display: flex;
      justify-content: space-between;
      align-items: flex-start;
      margin-bottom: 8px;
    }

    .item-name {
      font-size: 1.2em;
      font-weight: 600;
      color: ${theme.textColor};
    }

    .item-price {
      font-size: 1.1em;
      font-weight: bold;
      color: ${theme.primaryColor};
      white-space: nowrap;
      margin-left: 15px;
    }

    .item-description {
      color: ${theme.textColor};
      opacity: 0.8;
      line-height: 1.5;
    }

    .menu-footer {
      margin-top: 60px;
      padding-top: 30px;
      border-top: 2px solid ${theme.primaryColor};
      text-align: center;
      font-size: 0.9em;
      color: ${theme.textColor};
      opacity: 0.7;
    }

    @media print {
      body {
        padding: 20px;
      }

      .category {
        page-break-inside: avoid;
      }

      .menu-item {
        page-break-inside: avoid;
      }
    }
  </style>
</head>
<body>
  <div class="menu-header">
    ${logoUrl && includeImages ? `<img src="${logoUrl}" alt="${restaurantName}">` : ''}
    <h1>${restaurantName}</h1>
  </div>

  ${categories.map(category => `
    <div class="category">
      <div class="category-header">
        ${category.name}
      </div>

      ${category.items
        .filter(item => item.isAvailable)
        .map(item => `
          <div class="menu-item">
            ${item.imageUrl && includeImages ? `<img src="${item.imageUrl}" alt="${item.name}" class="item-image">` : ''}
            <div class="item-content">
              <div class="item-header">
                <span class="item-name">${item.name}</span>
                ${includePrices ? `<span class="item-price">$${item.price.toFixed(2)}</span>` : ''}
              </div>
              ${item.description && includeDescriptions ? `<div class="item-description">${item.description}</div>` : ''}
            </div>
          </div>
        `).join('')}
    </div>
  `).join('')}

  <div class="menu-footer">
    Generated on ${new Date().toLocaleDateString()} | ${restaurantName}
  </div>
</body>
</html>
    `

    return html
  }

  const exportToPDF = async (
    categories: MenuCategory[],
    theme: TemplateTheme,
    restaurantName: string,
    logoUrl?: string,
    options?: Partial<ExportOptions>
  ): Promise<void> => {
    const defaultOptions: ExportOptions = {
      format: 'pdf',
      orientation: 'portrait',
      paperSize: 'a4',
      includeImages: true,
      includePrices: true,
      includeDescriptions: true,
      fileName: `${restaurantName.replace(/[^a-z0-9]/gi, '_')}_menu.pdf`
    }

    const exportOptions = { ...defaultOptions, ...options }

    const html = generateMenuHTML(
      categories,
      theme,
      restaurantName,
      logoUrl,
      exportOptions
    )

    // Create a temporary element to hold the HTML
    const element = document.createElement('div')
    element.innerHTML = html
    element.style.width = '210mm' // A4 width

    const pdfOptions = {
      margin: [10, 10, 10, 10],
      filename: exportOptions.fileName,
      image: { type: 'jpeg', quality: 0.98 },
      html2canvas: {
        scale: 2,
        useCORS: true,
        logging: false
      },
      jsPDF: {
        unit: 'mm',
        format: exportOptions.paperSize,
        orientation: exportOptions.orientation
      }
    }

    try {
      await html2pdf().set(pdfOptions).from(element).save()
    } catch (error) {
      console.error('Failed to generate PDF:', error)
      throw error
    }
  }

  const exportToHTML = (
    categories: MenuCategory[],
    theme: TemplateTheme,
    restaurantName: string,
    logoUrl?: string,
    options?: Partial<ExportOptions>
  ): void => {
    const html = generateMenuHTML(
      categories,
      theme,
      restaurantName,
      logoUrl,
      options
    )

    const fileName = options?.fileName || `${restaurantName.replace(/[^a-z0-9]/gi, '_')}_menu.html`

    const blob = new Blob([html], { type: 'text/html' })
    const url = URL.createObjectURL(blob)
    const link = document.createElement('a')
    link.href = url
    link.download = fileName
    link.click()
    URL.revokeObjectURL(url)
  }

  const printMenu = (
    categories: MenuCategory[],
    theme: TemplateTheme,
    restaurantName: string,
    logoUrl?: string,
    options?: Partial<ExportOptions>
  ): void => {
    const html = generateMenuHTML(
      categories,
      theme,
      restaurantName,
      logoUrl,
      options
    )

    const printWindow = window.open('', '_blank')
    if (printWindow) {
      printWindow.document.write(html)
      printWindow.document.close()
      printWindow.onload = () => {
        printWindow.print()
      }
    }
  }

  return {
    exportToPDF,
    exportToHTML,
    printMenu,
    generateMenuHTML
  }
}
