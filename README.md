# External merge sort implementation.
### Sorting Criteria:
1. First, sort by the **string part** (alphabetically).
2. If two lines have the **same string**, sort by the **number** (ascending).

### FileCreationDemo is used to generate a file.
Each line follows the format: <Number>. <String>
Line generation options:
1. Words set count
2. Max word length
3. Max word count per line.

File generation options:
1. File size, MB
2. Batch size, bytes,
3. Threads count

### FileSorting demo is used to sort generated file
You can specify a file name and line count per chunk.

### FileSortingTaskTests is used to verify file created and sorted correctly.
