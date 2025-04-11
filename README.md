# Creating PDF Files From Html Using Playwright.NET

Works well and works fast up to at least 10k lines. Whilst it likely remains faster than IronPDF for bigger files (40k lines took about 50s), the memory usage of the Chrome headless browser becomes a problem. At 10k lines the memory usage spikes by roughly 1GB for a few seconds and then immediately drops back, but this does not scale linearly:

| Number of Lines | Approximate Memory Usage Spike |
|-----------------|--------------------------------|
| 10,000 | 1GB |
| 20,000 | 5GB |
| 30,000 | 8GB |
| 40,000 | 12GB |
| 50,000 | Out of Memory |

At 50,000 lines, the memory usage of Chrome exceeded 18-19GB whilst attempting to print to PDF which was the avilable memory on a 32GB machine running the tests, and crashed. This happens if you display the HTML in a normal Chrome borwser and try to print it as well. At this point, we could explore the possibility of splitting the input into smaller parts and recombining. The problem here is that this service is intended to take arbitrary HTML and render it in a PDF: This means that th service knows nothing of the content and it will split it up in a completely arbitarry fashion (maybe based on number of bytes or whatever), meaning that each chunk is no longer a valid HTML document and will not render properly.

It is worth noting that IronPDF, while more memory efficient, is noted as being CPU hungry. However, even with adequate CPU resource which isn't anywhere near fully utilised, it is very slow AND produces output files that are of the order of 7-10 times the size of those produced by Chrome when running direct comparisons with the same input files. It is unlikely to scale to bigger files any better than Playwright/Chromium does. Since we may need to produce files totalling 100,000 or more lines, we will need to think of a suitable strategy, such as giving the service paged input which it can then combine after rendering. This way the client can control the splitting of content and still guarantee that the output will make sense. It would be a compromise, but in the majority of use cases we would not need to split the input: Since this is an intended as an internal service where the consumers are known to us we can make the terms of use explicitly clear. We could even consider rejecting input that is phsyically larger than perhaps 4MB (roughly the size of the 20,000 line example file we have been using) as a bad request.
