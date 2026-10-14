interface PagerProps {
  pageNumber: number
  totalPages: number
  totalCount: number
  onChange: (pageNumber: number) => void
}

export function Pager({ pageNumber, totalPages, totalCount, onChange }: PagerProps) {
  const pages = Math.max(totalPages, 1)
  const link = (label: string, target: number, enabled: boolean) =>
    enabled ? (
      <a href="#" onClick={e => { e.preventDefault(); onChange(target) }}>{label}</a>
    ) : (
      <span className="disabled">{label}</span>
    )

  return (
    <div className="pager">
      Page {pageNumber} of {pages} ({totalCount} records)
      {link('<< First', 1, pageNumber > 1)}
      {link('< Previous', pageNumber - 1, pageNumber > 1)}
      {link('Next >', pageNumber + 1, pageNumber < pages)}
      {link('Last >>', pages, pageNumber < pages)}
    </div>
  )
}
