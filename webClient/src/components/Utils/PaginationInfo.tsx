import { forwardRef } from 'react';

interface IPaginationInfoProps {
  isLoading: boolean;
  isFetchingNextPage: boolean;
  hasNextPage: boolean;
  emptyList: boolean;
}

const PaginationInfo = forwardRef<HTMLDivElement, IPaginationInfoProps>(
  ({ isLoading, isFetchingNextPage, hasNextPage, emptyList }, ref) => {
    return (
      <div ref={ref} className="w-full py-4 text-center text-gray-600">
        {isLoading ? (
          <div>Loading content...</div>
        ) : isFetchingNextPage ? (
          <div>Loading more...</div>
        ) : hasNextPage ? (
          <div>Scroll for more</div>
        ) : emptyList ? (
          <div>There is no more content available</div>
        ) : (
          <div>No content found</div>
        )}
      </div>
    );
  }
);

PaginationInfo.displayName = 'PaginationInfo';

export default PaginationInfo;
