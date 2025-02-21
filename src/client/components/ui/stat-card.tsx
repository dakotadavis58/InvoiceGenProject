interface StatCardProps {
  amount: string;
  label: string;
}

export const StatCard = ({ amount, label }: StatCardProps) => {
  return (
    <div className="p-6 rounded-lg border">
      <h3 className="text-3xl font-bold text-blue-600">{amount}</h3>
      <p className="text-gray-600">{label}</p>
    </div>
  );
};
