import type { TodoStatistics } from '../api/types'

interface StatsPanelProps {
  statistics: TodoStatistics
}

export function StatsPanel({ statistics }: StatsPanelProps) {
  const cards = [
    { label: 'Total', value: statistics.totalCount, className: '' },
    { label: 'Pending', value: statistics.pendingCount, className: 'pending' },
    { label: 'Completed', value: statistics.completedCount, className: 'completed' },
    { label: 'Overdue', value: statistics.overdueCount, className: 'overdue' },
  ]

  return (
    <section className="stats-panel" aria-label="Statistics">
      {cards.map((card) => (
        <div key={card.label} className={`stat-card ${card.className}`.trim()}>
          <span className="stat-number">{card.value}</span>
          <span className="stat-label">{card.label}</span>
        </div>
      ))}
    </section>
  )
}
