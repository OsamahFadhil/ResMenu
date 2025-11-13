import html2canvas from 'html2canvas'
import jsPDF from 'jspdf'

export const useDesignerExport = () => {
  /**
   * Capture canvas element as image
   */
  const captureCanvas = async (element: HTMLElement): Promise<HTMLCanvasElement> => {
    try {
      const canvas = await html2canvas(element, {
        backgroundColor: null,
        scale: 2, // Higher quality
        useCORS: true,
        logging: false,
        allowTaint: true
      })
      return canvas
    } catch (error) {
      console.error('Error capturing canvas:', error)
      throw new Error('Failed to capture canvas')
    }
  }

  /**
   * Export as PNG image
   */
  const exportAsPNG = async (element: HTMLElement, filename: string = 'menu-design.png') => {
    try {
      const canvas = await captureCanvas(element)

      // Convert canvas to blob
      canvas.toBlob((blob) => {
        if (!blob) {
          throw new Error('Failed to create image blob')
        }

        // Create download link
        const url = URL.createObjectURL(blob)
        const link = document.createElement('a')
        link.href = url
        link.download = filename
        document.body.appendChild(link)
        link.click()
        document.body.removeChild(link)
        URL.revokeObjectURL(url)
      }, 'image/png', 1.0)

      return { success: true, message: 'PNG exported successfully' }
    } catch (error) {
      console.error('Error exporting PNG:', error)
      return { success: false, message: 'Failed to export PNG' }
    }
  }

  /**
   * Export as PDF document
   */
  const exportAsPDF = async (element: HTMLElement, filename: string = 'menu-design.pdf') => {
    try {
      const canvas = await captureCanvas(element)
      const imgData = canvas.toDataURL('image/png', 1.0)

      // Calculate PDF dimensions (A4 size: 210mm × 297mm)
      const imgWidth = canvas.width
      const imgHeight = canvas.height
      const ratio = imgHeight / imgWidth

      // Use custom size based on canvas dimensions
      const pdfWidth = 210 // mm
      const pdfHeight = pdfWidth * ratio

      const pdf = new jsPDF({
        orientation: pdfHeight > pdfWidth ? 'portrait' : 'landscape',
        unit: 'mm',
        format: [pdfWidth, pdfHeight]
      })

      pdf.addImage(imgData, 'PNG', 0, 0, pdfWidth, pdfHeight, undefined, 'FAST')
      pdf.save(filename)

      return { success: true, message: 'PDF exported successfully' }
    } catch (error) {
      console.error('Error exporting PDF:', error)
      return { success: false, message: 'Failed to export PDF' }
    }
  }

  /**
   * Export as SVG (simplified version - exports as PNG embedded in SVG)
   */
  const exportAsSVG = async (element: HTMLElement, filename: string = 'menu-design.svg') => {
    try {
      const canvas = await captureCanvas(element)
      const imgData = canvas.toDataURL('image/png', 1.0)

      // Create SVG with embedded PNG
      const svgContent = `<?xml version="1.0" encoding="UTF-8"?>
<svg width="${canvas.width}" height="${canvas.height}" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink">
  <image width="${canvas.width}" height="${canvas.height}" xlink:href="${imgData}"/>
</svg>`

      // Create download link
      const blob = new Blob([svgContent], { type: 'image/svg+xml' })
      const url = URL.createObjectURL(blob)
      const link = document.createElement('a')
      link.href = url
      link.download = filename
      document.body.appendChild(link)
      link.click()
      document.body.removeChild(link)
      URL.revokeObjectURL(url)

      return { success: true, message: 'SVG exported successfully' }
    } catch (error) {
      console.error('Error exporting SVG:', error)
      return { success: false, message: 'Failed to export SVG' }
    }
  }

  /**
   * Print the design
   */
  const printDesign = async (element: HTMLElement) => {
    try {
      const canvas = await captureCanvas(element)
      const imgData = canvas.toDataURL('image/png', 1.0)

      // Create print window
      const printWindow = window.open('', '_blank')
      if (!printWindow) {
        throw new Error('Failed to open print window')
      }

      printWindow.document.write(`
        <!DOCTYPE html>
        <html>
          <head>
            <title>Print Menu Design</title>
            <style>
              @media print {
                body {
                  margin: 0;
                  padding: 0;
                }
                img {
                  max-width: 100%;
                  height: auto;
                  display: block;
                  margin: 0 auto;
                  page-break-inside: avoid;
                }
              }
              body {
                margin: 0;
                padding: 20px;
                display: flex;
                justify-content: center;
                align-items: center;
                min-height: 100vh;
              }
            </style>
          </head>
          <body>
            <img src="${imgData}" alt="Menu Design" />
            <script>
              window.onload = function() {
                window.print();
                window.onafterprint = function() {
                  window.close();
                }
              }
            </script>
          </body>
        </html>
      `)
      printWindow.document.close()

      return { success: true, message: 'Print dialog opened' }
    } catch (error) {
      console.error('Error printing:', error)
      return { success: false, message: 'Failed to open print dialog' }
    }
  }

  /**
   * Export design data as JSON
   */
  const exportAsJSON = (projectData: any, filename: string = 'menu-design.json') => {
    try {
      const json = JSON.stringify(projectData, null, 2)
      const blob = new Blob([json], { type: 'application/json' })
      const url = URL.createObjectURL(blob)
      const link = document.createElement('a')
      link.href = url
      link.download = filename
      document.body.appendChild(link)
      link.click()
      document.body.removeChild(link)
      URL.revokeObjectURL(url)

      return { success: true, message: 'JSON exported successfully' }
    } catch (error) {
      console.error('Error exporting JSON:', error)
      return { success: false, message: 'Failed to export JSON' }
    }
  }

  return {
    captureCanvas,
    exportAsPNG,
    exportAsPDF,
    exportAsSVG,
    printDesign,
    exportAsJSON
  }
}
